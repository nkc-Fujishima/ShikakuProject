using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/PatrolEnemy", fileName = "PatrolEnemyParameter")]
public class PatrolEnemyParameterData : EnemyParameterDataBase
{
    [Header("����X�e�[�g�̕ϐ�")]
    [Tooltip("���񎞂̈ړ����x")] public float MoveSpeed_Patrol;
    [Tooltip("���񎞂̐��񑬓x")] public float RotateSpeed_Patrol;
    [Tooltip("���񎞂̑ҋ@����")] public float IdleTime;

}