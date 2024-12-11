using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/PatrolEnemy", fileName = "PatrolEnemyParameter")]
public class PatrolEnemyParameterData : EnemyParameterDataBase
{
    [Header("巡回ステートの変数")]
    [Tooltip("巡回時の移動速度")] public float MoveSpeed_Patrol;
    [Tooltip("巡回時の旋回速度")] public float RotateSpeed_Patrol;
    [Tooltip("巡回時の待機時間")] public float IdleTime;

}