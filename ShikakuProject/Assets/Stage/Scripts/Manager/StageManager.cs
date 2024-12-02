using R3;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private enum SceneType
    {
        Title,
        Game
    }

    private enum GameType
    {
        AllKill,
        ReachToGoal,
        Survival
    }

    // フジシマ追加 2024/10/04--------------------------------------------

    [Header("数値設定")]
    [Tooltip("シーンタイプ設定"), SerializeField]
    private SceneType _sceneType = SceneType.Game;
    [Tooltip("ゲームタイプ設定(SceneType : Title の場合無効)"), SerializeField]
    private GameType gameType = GameType.AllKill;
    [Tooltip("ゲーム内での制限時間"), SerializeField] float timeLimit;
    [Tooltip("ゲーム開始するまでの暗転時間(ミリ秒)"), SerializeField] int waitStartTime;

    // -------------------------------------------------------------------

    [Header("オブジェクト設定")]
    [SerializeField]
    private StageGenerateData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;

    [SerializeField]
    private UIStartSetting _uiClearTarget;

    [SerializeField]
    private UIResult _uiResult;

    [SerializeField]
    private StageSelectData _stageSelectData;

    // フジシマ追加 11/06---------------------------------
    [SerializeField] StageSelectData StageSelectData;
    [SerializeField] SceneChangeShaderController _sceneChangeShaderController;
    [SerializeField] GameStartShaderController _gameStartShaderController;
    //----------------------------------------------------


    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private EnemyManager _enemyManager;

    private BulletSpawnManager _bulletSpawnManager;


    public PlayerManager PlayerManager => _playerManager;
    public BulletManager BulletManager => _bulletManager;
    public EnemyManager EnemyManager => _enemyManager;

    // 残り時間をカウントする変数
    [HideInInspector]
    public TimeRemainingCounter TimeCounter;

    // ゲームが動いているかの変数
    public ReactiveProperty<bool> IsPlaying = new(false);

    private PlayerInput uiInput = null;

    private CancellationTokenSource cts = null;

    private bool isTutorial = false;


    private async void Awake()
    {
        switch (_sceneType)
        {

            case SceneType.Title:

                // 各種マネージャーの初期化
                InitializeManagers();

                // ステージの生成とマネージャーのセットアップ
                GenerateStage(_sceneType);

                // ナビメッシュのベイク
                BakeNavMesh();

                // バレットスポーンマネージャーのセットアップ
                SetupBulletSpawnManager();

                // エネミーのスタート
                StartEnemies();

                break;

            case SceneType.Game:
                cts = new CancellationTokenSource();

                // このスクリプトのセットアップ
                SetUp();

                // 各種マネージャーの初期化
                InitializeManagers();

                // ステージの生成とマネージャーのセットアップ
                GenerateStage(_sceneType);

                // ナビメッシュのベイク
                BakeNavMesh();

                // バレットスポーンマネージャーのセットアップ
                SetupBulletSpawnManager();

                // タイマーを設定
                InitializeTimeCounter();

                // フェード用UIセットアップ
                SetUpFadeUI();

                
                await UniTask.Delay(waitStartTime);

                // ゲーム開始時フェードイン
                await FadeInAsync();

                // ステージ目標表示、ボタン入力待ち
                await UntilPressButtonAsync();

                // ゲームスタート
                IsPlaying.Value = true;

                // エネミーのスタート
                StartEnemies();

                // プレイヤーのスタート
                StartPlayer();

                // タイマー関連を設定
                if (!isTutorial)
                    TimeCounter.OnResume();

                break;
        }

    }

    private void Update()
    {
        // タイマーを動かす
        TimeCounter?.OnUpdate();
    }

    //------------------------------------------------------------------------------------------------------
    // このスクリプトのセットアップ
    private void SetUp()
    {
        uiInput = GetComponent<PlayerInput>();
    }

    //------------------------------------------------------------------------------------------------------
    // 各種マネージャーの初期化
    private void InitializeManagers()
    {
        _playerManager = new PlayerManager();
        _bulletManager = new BulletManager();
        _enemyManager = new EnemyManager();
    }

    //------------------------------------------------------------------------------------------------------
    // ステージの生成とマネージャーのセットアップ
    private void GenerateStage(SceneType sceneType)
    {
        _data.OnStart(transform);

        bool isMultipleTypes = sceneType == SceneType.Game;

        int stageNumber = isMultipleTypes ? _stageSelectData.StageSelectNumber : 0;

        _data.StageGenerete(stageNumber, out GameObject[] enemyObjects, out GameObject playerObject, out isTutorial);


        // エネミーマネージャーのセットアップ
        SetupEnemyManager(enemyObjects);

        // プレイヤーマネージャーのセットアップ
        SetupPlayerManager(playerObject);

        // バレットマネージャーのセットアップ
        SetupBulletManager();
    }

    // バレットマネージャーのセットアップ
    private void SetupBulletManager()
    {
        _bulletManager.SetIEnemyListProvider(_enemyManager);
    }

    // エネミーマネージャーのセットアップ
    private void SetupEnemyManager(GameObject[] enemyObjects)
    {
        foreach (var enemyObject in enemyObjects)
        {
            _enemyManager.AddEnemy(enemyObject.GetComponent<EnemyControllerBase>());
        }
        _enemyManager.OnClearHundle += StageClear;
    }

    // プレイヤーマネージャーのセットアップ
    private void SetupPlayerManager(GameObject playerObject)
    {
        var playerController = playerObject.GetComponent<PlayerCharaController>();

        _playerManager.SetPlayerCharaController(playerController);
        _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
    }

    //------------------------------------------------------------------------------------------------------
    // ナビメッシュのベイク
    private void BakeNavMesh()
    {
        _navMesh.BuildNavMesh();
    }

    //------------------------------------------------------------------------------------------------------
    // バレットスポーンマネージャーのセットアップ
    private void SetupBulletSpawnManager()
    {
        _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();

        var playerController = _playerManager.PlayerCharaController;

        _bulletSpawnManager.SetPlayerCharaController(playerController);
        _bulletSpawnManager.SetIBulletSpawn(_bulletManager);
    }

    //------------------------------------------------------------------------------------------------------
    // エネミーのスタート
    private void StartEnemies()
    {
        _enemyManager.ExexuteEnemyStartMethod();
    }


    //------------------------------------------------------------------------------------------------------
    // プレイヤーのスタート
    private void StartPlayer()
    {
        _playerManager.ExecutePlayerStart();
    }

    //------------------------------------------------------------------------------------------------------
    // 時間をカウントするクラスを宣言する
    private void InitializeTimeCounter()
    {
        TimeCounter = new();
        if (isTutorial)
            TimeCounter.SetTimer(100);
        else
            TimeCounter.SetTimer(timeLimit);

        TimeCounter.OnTimeUpEvent += _playerManager.TimeUp;
        TimeCounter.OnTimeUpEvent += GameOver;
    }



    //------------------------------------------------------------------------------------------------------
    // リザルトに遷移する処理
    bool _isResult = false;

    private async void StageClear()
    {
        if (_isResult) return;
        _isResult = true;

        _enemyManager.OnClearHundle -= StageClear;

        // ゲームが終了したときに呼ぶ関数を呼ぶ
        OnResult();

        bool isNextStage = false;

        if (StageSelectData.StageSelectNumber >= StageSelectData.StageCountMax)
        {
            _uiResult.HideNextAction1();
        }
        await _uiResult.OpenGameClearUI();
        await UniTask.WaitUntil(() =>
        {
            if (uiInput.actions["Dicision"].WasPressedThisFrame())
            {
                if (StageSelectData.StageSelectNumber >= StageSelectData.StageCountMax) return false;

                isNextStage = true;
                return true;
            }
            if (uiInput.actions["Cancel"].WasPressedThisFrame())
            {
                isNextStage = false;
                return true;
            }
            return false;
        }, cancellationToken: cts.Token);
        await _uiResult.CloseResultUI();

        if (isNextStage)
        {

            StageSelectData.StageSelectNumber += 1;

            await FadeOutAsync();

            SceneManager.LoadScene("GameScene");
        }
        else if (!isNextStage)
        {
            await FadeOutAsync();

            SceneManager.LoadScene("StageSelect");
        }
    }

    private async void GameOver()
    {
        if (_isResult) return;
        _isResult = true;

        TimeCounter.OnTimeUpEvent -= GameOver;
        TimeCounter.OnTimeUpEvent -= _playerManager.TimeUp;

        // ゲームが終了したときに呼ぶ関数を呼ぶ
        OnResult();

        bool isRetry = false;

        await _uiResult.OpenGameFailedUI();
        await UniTask.WaitUntil(() =>
        {
            if (uiInput.actions["Dicision"].WasPressedThisFrame())
            {
                isRetry = true;
                return true;
            }
            if (uiInput.actions["Cancel"].WasPressedThisFrame())
            {
                isRetry = false;
                return true;
            }
            return false;
        }, cancellationToken: cts.Token);
        await _uiResult.CloseResultUI();

        if (isRetry)
        {
            await FadeOutAsync();

            SceneManager.LoadScene("GameScene");
        }
        else if (!isRetry)
        {
            await FadeOutAsync();

            SceneManager.LoadScene("StageSelect");
        }
    }

    // ゲームが終了したときに呼ばれる
    private void OnResult()
    {
        // プレイヤーを止める
        _playerManager.StopPlayer();

        // タイマーを止める
        TimeCounter.OnPause();

        // プレイが終了
        IsPlaying.Value = false;
    }

    // UIのセットアップ--------------------------------------------------------------------------------------------------------
    private void SetUpFadeUI()
    {
        _gameStartShaderController.SetUp(_playerManager.GetPlayerOnScreenPos());
        _sceneChangeShaderController.SetUp();
        _sceneChangeShaderController.SetFadeValueMin();
    }

    // ステージ開始画面フェードイン------------------------------------------------------------------------------------
    private async UniTask FadeInAsync()
    {
        await _gameStartShaderController.FadeIn();
    }

    // ステージ終了時画面フェードアウト-------------------------------------------------------------------------------
    private async UniTask FadeOutAsync()
    {
        await _sceneChangeShaderController.FadeOut();
    }

    // ボタン入力してゲーム開始を待つ処理-------------------------------------------------------------------------------
    private async UniTask UntilPressButtonAsync()
    {
        await _uiClearTarget.OpenGameStartUI((int)gameType);
        await UniTask.WaitUntil(() => uiInput.actions["Dicision"].WasPressedThisFrame(), cancellationToken: cts.Token);
        await _uiClearTarget.CloseGameStartUI();
    }

    private void OnDestroy()
    {
        cts?.Cancel();
    }
}
