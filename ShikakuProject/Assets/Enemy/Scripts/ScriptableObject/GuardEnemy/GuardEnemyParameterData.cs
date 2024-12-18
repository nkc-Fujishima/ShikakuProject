using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/GuardEnemy", fileName = "GuardEnemyParameter")]
public class GuardEnemyParameterData : EnemyParameterDataBase
{
    [Header("����X�e�[�g�̕ϐ�")]
    [Tooltip("���񎞂̈ړ����x")] public float MoveSpeed_Patrol;
    [Tooltip("���񎞂̐��񑬓x")] public float RotateSpeed_Patrol;

    [Header("�����X�e�[�g�̕ϐ�")]
    [Tooltip("�������̐��񑬓x")] public float RotateSpeed_Adjustment;
}
