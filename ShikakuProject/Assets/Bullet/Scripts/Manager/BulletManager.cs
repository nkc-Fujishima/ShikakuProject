using UnityEngine;

public class BulletManager : IBulletSpawn
{

    private IEnemyListProvider _enemyListProvider;

    public void SetIEnemyListProvider(IEnemyListProvider enemyListProvider)
    {
        _enemyListProvider = enemyListProvider;
    }


    //----------------------------------------------------------------------------------
    // IBulletSpawn
    public void BulletSpawn(BulletControllerBase bullet)
    {
        Transform closeEnemyTransform = null;

        float distance = float.MaxValue;

        foreach (var enemy in _enemyListProvider.EnemyList)
        {
            if (!enemy.gameObject.activeSelf) continue;

            float distanceEnemyToBullet = (enemy.transform.position - bullet.transform.position).magnitude;

            if (distance <= distanceEnemyToBullet) continue;

            distance = distanceEnemyToBullet;
            closeEnemyTransform = enemy.transform;
        }

        bullet.OnEnter(closeEnemyTransform);
    }
}
