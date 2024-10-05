using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : IEnemyListProvider
{
    private List<EnemyControllerBase> enemyList = new List<EnemyControllerBase>();

    public List<EnemyControllerBase> EnemyList => enemyList;

    public event Action OnEnemyDestroyHundle = null;
    public event Action OnClearHundle = null;

    public void AddEnemy(EnemyControllerBase enemy)
    {
        if (enemy != null) enemyList.Add(enemy);
    }

    private void RemoveEnemy(EnemyControllerBase enemy)
    {
        enemyList.Remove(enemy);
        enemy.OnDestroyHundle -= RemoveEnemy;

        OnEnemyDestroyHundle?.Invoke();

        if (enemyList.Count == 0) OnClearHundle?.Invoke();
    }

    public void ExexuteEnemyStartMethod()
    {
        foreach(var enemy in enemyList)
        {
            enemy.OnDestroyHundle += RemoveEnemy;
            enemy.OnStart();
        }
    }

}
