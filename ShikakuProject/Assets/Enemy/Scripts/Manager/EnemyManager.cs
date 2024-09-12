using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : IEnemyListProvider
{
    private List<EnemyControllerBase> enemyList = new List<EnemyControllerBase>();

    public List<EnemyControllerBase> EnemyList => enemyList;

    public void AddEnemy(EnemyControllerBase enemy)
    {
        if (enemy != null) enemyList.Add(enemy);
    }
}
