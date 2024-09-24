using Unity.AI.Navigation;
using UnityEngine;

public class StageManager:MonoBehaviour
{
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;

    // �X�e�[�W�������߂�ϐ�
    // �Ⴄ�ꏊ���玝���Ă����������ŏ���
    [SerializeField]
    int _stageCount = 0;

    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private EnemyManager _enemyManager;


    private void Start()
    {
        // �e��}�l�[�W���[�C���X�^���X�𐶐�
        _playerManager = new();
        _bulletManager = new();
        _enemyManager = new();


        // �X�e�[�W����
        _data.OnStart(transform);
        _data.StageGenerete(_stageCount, out GameObject[] enemyObjects, out GameObject playerObject);


        // �e��}�l�[�W���[�Z�b�g�A�b�v
        // �X�e�[�W�W�F�l���[�^�̐��������}�l�[�W���[�Ɋi�[
        _bulletManager.SetIEnemyListProvider(_enemyManager);

        for (int i = 0; i < enemyObjects.Length; ++i)
            _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

        _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
        _playerManager.SetIBulletSpawn(_bulletManager);


        // �i�r���b�V�����x�C�N
        _navMesh.BuildNavMesh();


        // �Q�[���X�^�[�g


    }

}
