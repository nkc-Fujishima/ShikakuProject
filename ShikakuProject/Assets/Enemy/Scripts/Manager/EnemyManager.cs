using System;
using System.Collections.Generic;

public class EnemyManager : IEnemyListProvider
{
    private List<EnemyControllerBase> enemyList = new List<EnemyControllerBase>();

    public List<EnemyControllerBase> EnemyList => enemyList;

    public event Action OnEnemyDestroyHundle = null;
    public event Action OnClearHundle = null;

    // リストにエネミーを追加
    public void AddEnemy(EnemyControllerBase enemy)
    {
        if (enemy != null) enemyList.Add(enemy);
    }

    // エネミーリストから引数に送られたエネミーを削除
    private void RemoveEnemy(EnemyControllerBase enemy)
    {
        enemyList.Remove(enemy);
        enemy.OnDestroyHundle -= RemoveEnemy;

        OnEnemyDestroyHundle?.Invoke();

        // エネミーリスト内の要素が0になった場合、ゲームクリアイベントを発火
        if (enemyList.Count == 0) OnClearHundle?.Invoke();
    }

    // リストに登録されているエネミーのゲームスタート時の初期設定を起動
    public void ExexuteEnemyStartMethod()
    {
        foreach(var enemy in enemyList)
        {
            enemy.OnDestroyHundle += RemoveEnemy;
            enemy.OnStart();
        }
    }

}
