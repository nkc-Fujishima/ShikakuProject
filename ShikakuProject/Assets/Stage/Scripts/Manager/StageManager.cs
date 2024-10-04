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

    // フジシマ追加 2024/10/04--------------------------------------------

    [Header("数値設定")]
    [Tooltip("シーンタイプ設定"), SerializeField]
    SceneType sceneType = SceneType.Game;
    [Tooltip("ゲームタイプ設定(SceneType : Title の場合無効)"), SerializeField]
    GameType gameType = GameType.AllKill;

    // -------------------------------------------------------------------

    [Header("オブジェクト設定")]
    [SerializeField]
    private StageGenereteData _data;

    [SerializeField]
    private NavMeshSurface _navMesh;



    // ステージ数を決める変数
    // スクリプタブルオブジェクト内部のint変数を参照
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
                    // 各種マネージャーインスタンスを生成
                    _playerManager = new();
                    _bulletManager = new();
                    _enemyManager = new();


                    // ステージ生成
                    _data.OnStart(transform);
                    _data.StageGenerete(0, out GameObject[] enemyObjects, out GameObject playerObject);


                    // 各種マネージャーセットアップ
                    // ステージジェネレータの生成情報をマネージャーに格納
                    #region バレットマネージャーセットアップ
                    _bulletManager.SetIEnemyListProvider(_enemyManager);
                    #endregion



                    #region エネミーマネージャーセットアップ
                    for (int i = 0; i < enemyObjects.Length; ++i)
                        _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

                    _enemyManager.OnClearHundle += StageClear;
                    #endregion



                    #region プレイヤーマネージャーセットアップ
                    _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
                    #endregion



                    // ナビメッシュをベイク
                    _navMesh.BuildNavMesh();


                    // バレットのオブジェクトプールを設定
                    _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();
                    _bulletSpawnManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _bulletSpawnManager.SetIBulletSpawn(_bulletManager);


                    // エネミーのスタートを起動
                    _enemyManager.ExexuteEnemyStartMethod();
                    break;
                }

            case SceneType.Game:
                {
                    // 各種マネージャーインスタンスを生成
                    _playerManager = new();
                    _bulletManager = new();
                    _enemyManager = new();


                    // ステージ生成
                    _data.OnStart(transform);
                    _data.StageGenerete(_stageSelectData.StageSelectNumber, out GameObject[] enemyObjects, out GameObject playerObject);


                    // 各種マネージャーセットアップ
                    // ステージジェネレータの生成情報をマネージャーに格納
                    #region バレットマネージャーセットアップ
                    _bulletManager.SetIEnemyListProvider(_enemyManager);
                    #endregion



                    #region エネミーマネージャーセットアップ
                    for (int i = 0; i < enemyObjects.Length; ++i)
                        _enemyManager.AddEnemy(enemyObjects[i].GetComponent<EnemyControllerBase>());

                    _enemyManager.OnClearHundle += StageClear;
                    #endregion



                    #region プレイヤーマネージャーセットアップ
                    _playerManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _playerManager.OnDieHundle.Subscribe(_ => { GameOver(); }).AddTo(this);
                    #endregion



                    // ナビメッシュをベイク
                    _navMesh.BuildNavMesh();


                    // バレットのオブジェクトプールを設定
                    _bulletSpawnManager = gameObject.AddComponent<BulletSpawnManager>();
                    _bulletSpawnManager.SetPlayerCharaController(playerObject.GetComponent<PlayerCharaController>());
                    _bulletSpawnManager.SetIBulletSpawn(_bulletManager);


                    // エネミーのスタートを起動
                    _enemyManager.ExexuteEnemyStartMethod();


                    // ゲームスタート

                    break;
                }
        }

    }

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
