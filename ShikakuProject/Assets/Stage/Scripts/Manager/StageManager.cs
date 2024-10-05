using R3;
using Unity.AI.Navigation;
using UnityEngine;

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

    // -------------------------------------------------------------------

    [Header("オブジェクト設定")]
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;



    // ステージ数を決める変数
    // スクリプタブルオブジェクト内部のint変数を参照
    [SerializeField]
    private StageSelectData _stageSelectData;

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


    private void Awake()
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


                // タイマーを設定
                InitializeTimeCounter();

                // ゲームスタート

                // タイマーを動かす
                //TimeCounter.OnResume();

                break;
        }

    }

    private void Update()
    {
        // タイマーを動かす
        TimeCounter.OnUpdate();
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

        _data.StageGenerete(stageNumber, out GameObject[] enemyObjects, out GameObject playerObject);


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
    // 時間をカウントするクラスを宣言する
    private void InitializeTimeCounter()
    {

        //具体的な時間が決まってないというかまだ処理を作ってないので、５秒でひとまず仮置きしますうううううううううううううう
        int settingTime = 5;
        TimeCounter = new();
        TimeCounter.SetTimer(settingTime);


    }



    //------------------------------------------------------------------------------------------------------
    private void StageClear()
    {
        Debug.Log("ステージのクリア条件を達成したよ!");
        _enemyManager.OnClearHundle -= StageClear;
    }

    private void GameOver()
    {
        Debug.Log("ゲームオーバーだよ");
    }

}
