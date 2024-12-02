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

    // �t�W�V�}�ǉ� 2024/10/04--------------------------------------------

    [Header("���l�ݒ�")]
    [Tooltip("�V�[���^�C�v�ݒ�"), SerializeField]
    private SceneType _sceneType = SceneType.Game;
    [Tooltip("�Q�[���^�C�v�ݒ�(SceneType : Title �̏ꍇ����)"), SerializeField]
    private GameType gameType = GameType.AllKill;
    [Tooltip("�Q�[�����ł̐�������"), SerializeField] float timeLimit;
    [Tooltip("�Q�[���J�n����܂ł̈Ó]����(�~���b)"), SerializeField] int waitStartTime;

    // -------------------------------------------------------------------

    [Header("�I�u�W�F�N�g�ݒ�")]
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

    // �t�W�V�}�ǉ� 11/06---------------------------------
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

    // �c�莞�Ԃ��J�E���g����ϐ�
    [HideInInspector]
    public TimeRemainingCounter TimeCounter;

    // �Q�[���������Ă��邩�̕ϐ�
    public ReactiveProperty<bool> IsPlaying = new(false);

    private PlayerInput uiInput = null;

    private CancellationTokenSource cts = null;

    private bool isTutorial = false;


    private async void Awake()
    {
        switch (_sceneType)
        {

            case SceneType.Title:

                // �e��}�l�[�W���[�̏�����
                InitializeManagers();

                // �X�e�[�W�̐����ƃ}�l�[�W���[�̃Z�b�g�A�b�v
                GenerateStage(_sceneType);

                // �i�r���b�V���̃x�C�N
                BakeNavMesh();

                // �o���b�g�X�|�[���}�l�[�W���[�̃Z�b�g�A�b�v
                SetupBulletSpawnManager();

                // �G�l�~�[�̃X�^�[�g
                StartEnemies();

                break;

            case SceneType.Game:
                cts = new CancellationTokenSource();

                // ���̃X�N���v�g�̃Z�b�g�A�b�v
                SetUp();

                // �e��}�l�[�W���[�̏�����
                InitializeManagers();

                // �X�e�[�W�̐����ƃ}�l�[�W���[�̃Z�b�g�A�b�v
                GenerateStage(_sceneType);

                // �i�r���b�V���̃x�C�N
                BakeNavMesh();

                // �o���b�g�X�|�[���}�l�[�W���[�̃Z�b�g�A�b�v
                SetupBulletSpawnManager();

                // �^�C�}�[��ݒ�
                InitializeTimeCounter();

                // �t�F�[�h�pUI�Z�b�g�A�b�v
                SetUpFadeUI();

                
                await UniTask.Delay(waitStartTime);

                // �Q�[���J�n���t�F�[�h�C��
                await FadeInAsync();

                // �X�e�[�W�ڕW�\���A�{�^�����͑҂�
                await UntilPressButtonAsync();

                // �Q�[���X�^�[�g
                IsPlaying.Value = true;

                // �G�l�~�[�̃X�^�[�g
                StartEnemies();

                // �v���C���[�̃X�^�[�g
                StartPlayer();

                // �^�C�}�[�֘A��ݒ�
                if (!isTutorial)
                    TimeCounter.OnResume();

                break;
        }

    }

    private void Update()
    {
        // �^�C�}�[�𓮂���
        TimeCounter?.OnUpdate();
    }

    //------------------------------------------------------------------------------------------------------
    // ���̃X�N���v�g�̃Z�b�g�A�b�v
    private void SetUp()
    {
        uiInput = GetComponent<PlayerInput>();
    }

    //------------------------------------------------------------------------------------------------------
    // �e��}�l�[�W���[�̏�����
    private void InitializeManagers()
    {
        _playerManager = new PlayerManager();
        _bulletManager = new BulletManager();
        _enemyManager = new EnemyManager();
    }

    //------------------------------------------------------------------------------------------------------
    // �X�e�[�W�̐����ƃ}�l�[�W���[�̃Z�b�g�A�b�v
    private void GenerateStage(SceneType sceneType)
    {
        _data.OnStart(transform);

        bool isMultipleTypes = sceneType == SceneType.Game;

        int stageNumber = isMultipleTypes ? _stageSelectData.StageSelectNumber : 0;

        _data.StageGenerete(stageNumber, out GameObject[] enemyObjects, out GameObject playerObject, out isTutorial);


        // �G�l�~�[�}�l�[�W���[�̃Z�b�g�A�b�v
        SetupEnemyManager(enemyObjects);

        // �v���C���[�}�l�[�W���[�̃Z�b�g�A�b�v
        SetupPlayerManager(playerObject);

        // �o���b�g�}�l�[�W���[�̃Z�b�g�A�b�v
        SetupBulletManager();
    }

    // �o���b�g�}�l�[�W���[�̃Z�b�g�A�b�v
    private void SetupBulletManager()
    {
        _bulletManager.SetIEnemyListProvider(_enemyManager);
    }

    // �G�l�~�[�}�l�[�W���[�̃Z�b�g�A�b�v
    private void SetupEnemyManager(GameObject[] enemyObjects)
    {
        foreach (var enemyObject in enemyObjects)
        {
            _enemyManager.AddEnemy(enemyObject.GetComponent<EnemyControllerBase>());
        }
        _enemyManager.OnClearHundle += StageClear;
    }

    // �v���C���[�}�l�[�W���[�̃Z�b�g�A�b�v
    private void SetupPlayerManager(GameObject playerObject)
    {
        var playerController = playerObject.GetComponent<PlayerCharaController>();

        _playerManager.SetPlayerCharaController(playerController);
        _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
    }

    //------------------------------------------------------------------------------------------------------
    // �i�r���b�V���̃x�C�N
    private void BakeNavMesh()
    {
        _navMesh.BuildNavMesh();
    }

    //------------------------------------------------------------------------------------------------------
    // �o���b�g�X�|�[���}�l�[�W���[�̃Z�b�g�A�b�v
    private void SetupBulletSpawnManager()
    {
        _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();

        var playerController = _playerManager.PlayerCharaController;

        _bulletSpawnManager.SetPlayerCharaController(playerController);
        _bulletSpawnManager.SetIBulletSpawn(_bulletManager);
    }

    //------------------------------------------------------------------------------------------------------
    // �G�l�~�[�̃X�^�[�g
    private void StartEnemies()
    {
        _enemyManager.ExexuteEnemyStartMethod();
    }


    //------------------------------------------------------------------------------------------------------
    // �v���C���[�̃X�^�[�g
    private void StartPlayer()
    {
        _playerManager.ExecutePlayerStart();
    }

    //------------------------------------------------------------------------------------------------------
    // ���Ԃ��J�E���g����N���X��錾����
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
    // ���U���g�ɑJ�ڂ��鏈��
    bool _isResult = false;

    private async void StageClear()
    {
        if (_isResult) return;
        _isResult = true;

        _enemyManager.OnClearHundle -= StageClear;

        // �Q�[�����I�������Ƃ��ɌĂԊ֐����Ă�
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

        // �Q�[�����I�������Ƃ��ɌĂԊ֐����Ă�
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

    // �Q�[�����I�������Ƃ��ɌĂ΂��
    private void OnResult()
    {
        // �v���C���[���~�߂�
        _playerManager.StopPlayer();

        // �^�C�}�[���~�߂�
        TimeCounter.OnPause();

        // �v���C���I��
        IsPlaying.Value = false;
    }

    // UI�̃Z�b�g�A�b�v--------------------------------------------------------------------------------------------------------
    private void SetUpFadeUI()
    {
        _gameStartShaderController.SetUp(_playerManager.GetPlayerOnScreenPos());
        _sceneChangeShaderController.SetUp();
        _sceneChangeShaderController.SetFadeValueMin();
    }

    // �X�e�[�W�J�n��ʃt�F�[�h�C��------------------------------------------------------------------------------------
    private async UniTask FadeInAsync()
    {
        await _gameStartShaderController.FadeIn();
    }

    // �X�e�[�W�I������ʃt�F�[�h�A�E�g-------------------------------------------------------------------------------
    private async UniTask FadeOutAsync()
    {
        await _sceneChangeShaderController.FadeOut();
    }

    // �{�^�����͂��ăQ�[���J�n��҂���-------------------------------------------------------------------------------
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
