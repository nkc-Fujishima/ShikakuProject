using UnityEngine;
using UnityEngine.Events;

public class BulletSpawnManager : MonoBehaviour
{
    private class BulletObjectPool
    {
        private GameObject _obstaclePrefab;
        private int _activeObjectValues;

        public GameObject GetObstaclePrefab => _obstaclePrefab;
        public int ActiveObjectValues => _activeObjectValues;

        [HideInInspector]
        public ObjectPool.BulletObjectPool ObstaclePool;

        public void SetActiveObjectValues()
        {
            _activeObjectValues = OBSTACLE_PREFB_LENGTH;
        }

        public void SetBulletTypeCount(int typeCount)
        {
            _obstaclePrefab.GetComponent<IBulletManaged>().SetTypeCount(typeCount);
        }

        public void AddActiveObjectValues() { _activeObjectValues++; }
        public void DelActiveObjectValues() { _activeObjectValues--; }

        public GameObject SetObstacleObject { set { _obstaclePrefab = value; } }
    }

    private IBulletSpawn _bulletSpawn;
    private PlayerCharaController _playerController;

    private const int OBSTACLE_PREFB_LENGTH = 3;
    private BulletObjectPool[] _bulletPools;
    private int[] _activeObjectValues;

    private Vector3 _nextSpawnPoint = Vector3.zero;
    private Quaternion _nextSpawnRotate = Quaternion.identity;

    [HideInInspector]
    public UnityEvent<BulletControllerBase> OnBulletSpawn = new();


    //-------------------------------------------------------------------------------------------
    // �o���b�g�𐶐�
    public void SpawnBullet()
    {
        BulletObjectPool pool = _bulletPools[_playerController.GetSelectBulletType];
        GameObject bulletSegment = pool.ObstaclePool.Get();

        if (bulletSegment != null)
        {
            Transform bulletPoint = _playerController.Datas.SpawnBulletPoint;
            bulletSegment.transform.SetPositionAndRotation(bulletPoint.position, bulletPoint.rotation);
            bulletSegment.SetActive(true);

            // _activeObjectValues�Ɂ[�P����
            pool.DelActiveObjectValues();

            BulletControllerBase bulletPrefab = bulletSegment.GetComponent<BulletControllerBase>();
            OnBulletSpawn.Invoke(bulletPrefab);
            bulletPrefab.OnBulletDestroy.AddListener(ReturnBullet);
        }
    }

    //-------------------------------------------------------------------------------------------
    // �g�p���I������o���b�g���A�N�e�B�u�ɂ���
    public void ReturnBullet(GameObject bulletSegment)
    {
        BulletControllerBase bulletPrefab = bulletSegment.GetComponent<BulletControllerBase>();
        bulletPrefab.OnBulletDestroy.RemoveListener(ReturnBullet);

        int typeCount = bulletSegment.GetComponent<IBulletManaged>().GetTypeCount();


        BulletObjectPool pool = _bulletPools[typeCount];

        pool.ObstaclePool.Return(bulletSegment);

        // _activeObjectValues�Ɂ{�P����
        pool.AddActiveObjectValues();
    }


    //-------------------------------------------------------------------------------------------
    // �v���C���[���擾
    public void SetPlayerCharaController(PlayerCharaController player)
    {
        _playerController = player;
        _playerController.Datas.OnBulletSpawn.AddListener(SpawnBullet);

        SetBulletPool();
        OnStartInstance();
    }

    // �o���b�g�̃I�u�W�F�N�g�v�[���̃I�u�W�F�N�g��ݒ肷��
    private void SetBulletPool()
    {
        BulletControllerBase[] bulletPrefabs = _playerController.GetBulletPrefabs;
        _bulletPools = new BulletObjectPool[bulletPrefabs.Length];

        for (int i = 0; i < _bulletPools.Length; ++i)
        {
            _bulletPools[i] = new BulletObjectPool();
            _bulletPools[i].SetObstacleObject = bulletPrefabs[i].gameObject;
        }
    }

    // �I�u�W�F�N�g�v�[���̏�����
    private void OnStartInstance()
    {
        for (int BulletTypeCount = 0; BulletTypeCount < _bulletPools.Length; BulletTypeCount++)
        {
            _bulletPools[BulletTypeCount].SetActiveObjectValues();
            _bulletPools[BulletTypeCount].SetBulletTypeCount(BulletTypeCount);

            _bulletPools[BulletTypeCount].ObstaclePool = gameObject.AddComponent<ObjectPool.BulletObjectPool>();
            _bulletPools[BulletTypeCount].ObstaclePool.Initialize(_bulletPools[BulletTypeCount].GetObstaclePrefab, OBSTACLE_PREFB_LENGTH);
        }
    }

    //-------------------------------------------------------------------------------------------
    // �o���b�g���������ꂽ���Ƃ�m�点�邽�߂̃N���X���擾
    public void SetIBulletSpawn(IBulletSpawn bulletSpawn)
    {
        _bulletSpawn = bulletSpawn;
    }
}