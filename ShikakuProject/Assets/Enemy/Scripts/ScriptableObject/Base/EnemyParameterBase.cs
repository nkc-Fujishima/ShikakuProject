using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParameterBase : ScriptableObject
{
    [Tooltip("移動速度")] public float MoveSpeed;
    [Tooltip("攻撃開始距離")] public float AttackRange;
    [Tooltip("警戒解除時間")] public float alertResetTime;

    [Tooltip("追跡対象リストのリフレッシュレート(高いほど低頻度)")] public ulong ListRefreshRate;
}
