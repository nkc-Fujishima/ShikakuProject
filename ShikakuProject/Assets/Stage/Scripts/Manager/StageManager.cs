using Unity.AI.Navigation;
using UnityEngine;

public class StageManager:MonoBehaviour
{
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;

    

    // ステージ数を決める変数
    // 違う場所から持ってきたいから後で消す
    [SerializeField]
    int _stageCount = 0;

    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private EnemyManager _enemyManager;

    private BulletSpawnManager _bulletSpawnManager;


    private void Awake()
    {
        // 各種マネージャーインスタンスを生成
        _playerManager = new();
        _bulletManager = new();
        _enemyManager = new();


        // ステージ生成
        _data.OnStart(transform);
        _data.StageGenerete(_stageCount, out GameObject[] enemyObjects, out GameObject playerObject);


        // 各種マネージャーセットアップ
        // ステージジェネレータの生成情報をマネージャーに格納
        _bulletManager.SetIEnemyListProvider(_enemyManager);

        for (int i = 0; i < enemyObjects.Length; ++i)
            _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

        _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());


        // ナビメッシュをベイク
        _navMesh.BuildNavMesh();


        // バレットのオブジェクトプールを設定
        _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();
        _bulletSpawnManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
        _bulletSpawnManager.SetIBulletSpawn(_bulletManager);


        // エネミーのスタートを起動
        _enemyManager.ExexuteEnemyStartMethod();


        // ゲームスタート

    }

}
