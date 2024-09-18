using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParameterBase : ScriptableObject
{
    [Tooltip("移動速度")] public float MoveSpeed;
    [Tooltip("攻撃開始距離")] public float AttackRange;
    [Tooltip("攻撃クールタイム")] public float AttackCoolTime;
    [Tooltip("警戒解除時間")] public float AlertResetTime;
    [Tooltip("ダメージを受けない角度")] public float InvincibleAngle;
    [Tooltip("旋回速度")] public float RotateSpeed;

    [Tooltip("追跡対象リストのリフレッシュレート(高いほど低頻度)")] public ulong ListRefreshRate;
}
