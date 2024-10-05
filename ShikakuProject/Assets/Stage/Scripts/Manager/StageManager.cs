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

    // �t�W�V�}�ǉ� 2024/10/04--------------------------------------------

    [Header("���l�ݒ�")]
    [Tooltip("�V�[���^�C�v�ݒ�"), SerializeField]
    private SceneType _sceneType = SceneType.Game;
    [Tooltip("�Q�[���^�C�v�ݒ�(SceneType : Title �̏ꍇ����)"), SerializeField]
    private GameType gameType = GameType.AllKill;

    // -------------------------------------------------------------------

    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;



    // �X�e�[�W�������߂�ϐ�
    // �X�N���v�^�u���I�u�W�F�N�g������int�ϐ����Q��
    [SerializeField]
    private StageSelectData _stageSelectData;

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


    private void Awake()
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


                // �^�C�}�[��ݒ�
                InitializeTimeCounter();

                // �Q�[���X�^�[�g

                // �^�C�}�[�𓮂���
                //TimeCounter.OnResume();

                break;
        }

    }

    private void Update()
    {
        // �^�C�}�[�𓮂���
        TimeCounter.OnUpdate();
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

        _data.StageGenerete(stageNumber, out GameObject[] enemyObjects, out GameObject playerObject);


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
    // ���Ԃ��J�E���g����N���X��錾����
    private void InitializeTimeCounter()
    {

        //��̓I�Ȏ��Ԃ����܂��ĂȂ��Ƃ������܂�����������ĂȂ��̂ŁA�T�b�łЂƂ܂����u�����܂�����������������������������
        int settingTime = 5;
        TimeCounter = new();
        TimeCounter.SetTimer(settingTime);


    }



    //------------------------------------------------------------------------------------------------------
    private void StageClear()
    {
        Debug.Log("�X�e�[�W�̃N���A������B��������!");
        _enemyManager.OnClearHundle -= StageClear;
    }

    private void GameOver()
    {
        Debug.Log("�Q�[���I�[�o�[����");
    }

}
