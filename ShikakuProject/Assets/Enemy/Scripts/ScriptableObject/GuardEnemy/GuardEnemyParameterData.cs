using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/GuardEnemy", fileName = "GuardEnemyParameter")]
public class GuardEnemyParameterData : EnemyParameterDataBase
{
    [Header("巡回ステートの変数")]
    [Tooltip("巡回時の移動速度")] public float MoveSpeed_Patrol;
    [Tooltip("巡回時の旋回速度")] public float RotateSpeed_Patrol;

    [Header("調整ステートの変数")]
    [Tooltip("調整時の旋回速度")] public float RotateSpeed_Adjustment;
}
