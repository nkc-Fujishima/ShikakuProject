using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyListProvider
{
    public List<EnemyControllerBase> EnemyList { get; }
}
