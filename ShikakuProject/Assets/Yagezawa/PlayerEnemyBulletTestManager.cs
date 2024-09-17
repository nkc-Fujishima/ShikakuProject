using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ただのテスト用のコード
// 生成スクリプトが作れたら消す
public class PlayerEnemyBulletTestManager : MonoBehaviour
{
    [SerializeField]
    PlayerCharaController plController;

    [SerializeField]
    EnemyControllerBase enemyController1;
    [SerializeField]
    EnemyControllerBase enemyController2;
    [SerializeField]
    EnemyControllerBase enemyController3;

    private void Start()
    {
        BulletManager bulletManager = new();
        PlayerManager playerManager = new();
        EnemyManager enemyManager = new();

        GameObject playerObj = Instantiate(plController.gameObject);

        GameObject enemyObj1 = null;
        GameObject enemyObj2 = null;
        GameObject enemyObj3 = null;

        if (enemyController1) enemyObj1 = Instantiate(enemyController1.gameObject, new Vector3(5, 0, 5), Quaternion.identity);
        if (enemyController2) enemyObj2 = Instantiate(enemyController2.gameObject, new Vector3(-5, 0, 5), Quaternion.identity);
        if (enemyController3) enemyObj3 = Instantiate(enemyController3.gameObject, new Vector3(-10, 0, 5), Quaternion.identity);

        bulletManager.SetIEnemyListProvider(enemyManager);
        playerManager.SetPlayerCharaController(playerObj.GetComponent<PlayerCharaController>());
        playerManager.SetIBulletSpawn(bulletManager);

        if (enemyObj1 != null) enemyManager.AddEnemy(enemyObj1.GetComponent<EnemyControllerBase>());
        if (enemyObj2 != null) enemyManager.AddEnemy(enemyObj2.GetComponent<EnemyControllerBase>());
        if (enemyObj3 != null) enemyManager.AddEnemy(enemyObj3.GetComponent<EnemyControllerBase>());
    }

}
