using R3;
using Unity.AI.Navigation;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    enum SceneType
    {
        Title,
        Game
    }

    enum GameType
    {
        AllKill,
        ReachToGoal,
        Survival
    }

    // �t�W�V�}�ǉ� 2024/10/04--------------------------------------------

    [Header("���l�ݒ�")]
    [Tooltip("�V�[���^�C�v�ݒ�"), SerializeField]
    SceneType sceneType = SceneType.Game;
    [Tooltip("�Q�[���^�C�v�ݒ�(SceneType : Title �̏ꍇ����)"), SerializeField]
    GameType gameType = GameType.AllKill;

    // -------------------------------------------------------------------

    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;



    // �X�e�[�W�������߂�ϐ�
    // �X�N���v�^�u���I�u�W�F�N�g������int�ϐ����Q��
    [SerializeField]
    StageSelectData _stageSelectData;

    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private EnemyManager _enemyManager;

    private BulletSpawnManager _bulletSpawnManager;


    private void Awake()
    {
        switch (sceneType)
        {

            case SceneType.Title:
                {
                    // �e��}�l�[�W���[�C���X�^���X�𐶐�
                    _playerManager = new();
                    _bulletManager = new();
                    _enemyManager = new();


                    // �X�e�[�W����
                    _data.OnStart(transform);
                    _data.StageGenerete(0, out GameObject[] enemyObjects, out GameObject playerObject);


                    // �e��}�l�[�W���[�Z�b�g�A�b�v
                    // �X�e�[�W�W�F�l���[�^�̐��������}�l�[�W���[�Ɋi�[
                    #region �o���b�g�}�l�[�W���[�Z�b�g�A�b�v
                    _bulletManager.SetIEnemyListProvider(_enemyManager);
                    #endregion



                    #region �G�l�~�[�}�l�[�W���[�Z�b�g�A�b�v
                    for (int i = 0; i < enemyObjects.Length; ++i)
                        _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

                    _enemyManager.OnClearHundle += StageClear;
                    #endregion



                    #region �v���C���[�}�l�[�W���[�Z�b�g�A�b�v
                    _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
                    #endregion



                    // �i�r���b�V�����x�C�N
                    _navMesh.BuildNavMesh();


                    // �o���b�g�̃I�u�W�F�N�g�v�[����ݒ�
                    _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();
                    _bulletSpawnManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _bulletSpawnManager.SetIBulletSpawn(_bulletManager);


                    // �G�l�~�[�̃X�^�[�g���N��
                    _enemyManager.ExexuteEnemyStartMethod();
                    break;
                }

            case SceneType.Game:
                {
                    // �e��}�l�[�W���[�C���X�^���X�𐶐�
                    _playerManager = new();
                    _bulletManager = new();
                    _enemyManager = new();


                    // �X�e�[�W����
                    _data.OnStart(transform);
                    _data.StageGenerete(_stageSelectData.StageSelectNumber, out GameObject[] enemyObjects, out GameObject playerObject);


                    // �e��}�l�[�W���[�Z�b�g�A�b�v
                    // �X�e�[�W�W�F�l���[�^�̐��������}�l�[�W���[�Ɋi�[
                    #region �o���b�g�}�l�[�W���[�Z�b�g�A�b�v
                    _bulletManager.SetIEnemyListProvider(_enemyManager);
                    #endregion



                    #region �G�l�~�[�}�l�[�W���[�Z�b�g�A�b�v
                    for (int i = 0; i < enemyObjects.Length; ++i)
                        _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

                    _enemyManager.OnClearHundle += StageClear;
                    #endregion



                    #region �v���C���[�}�l�[�W���[�Z�b�g�A�b�v
                    _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
                    #endregion



                    // �i�r���b�V�����x�C�N
                    _navMesh.BuildNavMesh();


                    // �o���b�g�̃I�u�W�F�N�g�v�[����ݒ�
                    _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();
                    _bulletSpawnManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _bulletSpawnManager.SetIBulletSpawn(_bulletManager);


                    // �G�l�~�[�̃X�^�[�g���N��
                    _enemyManager.ExexuteEnemyStartMethod();


                    // �Q�[���X�^�[�g

                    break;
                }
        }

    }

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
