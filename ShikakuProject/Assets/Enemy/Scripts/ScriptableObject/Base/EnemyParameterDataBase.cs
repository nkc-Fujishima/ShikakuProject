using UnityEngine;

public class EnemyParameterDataBase : ScriptableObject
{
    [Tooltip("移動速度")] public float MoveSpeed;
    [Tooltip("攻撃開始距離")] public float AttackRange;
    [Tooltip("攻撃クールタイム")] public float AttackCoolTime;
    [Tooltip("警戒解除時間")] public float AlertResetTime;
    [Tooltip("ダメージを受けない角度")] public float InvincibleAngle;
    [Tooltip("旋回速度")] public float RotateSpeed;

    [Tooltip("追跡対象リストのリフレッシュレート(高いほど低頻度)")] public ulong ListRefreshRate;

    [Tooltip("倒されたときに吹き飛ばされる力")] public float DownForcePower;
    [Tooltip("倒された後の消えるまでの時間")] public float ToDestroyTime;
}
