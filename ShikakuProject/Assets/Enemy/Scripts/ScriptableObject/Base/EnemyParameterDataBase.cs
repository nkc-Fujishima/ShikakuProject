using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParameterDataBase : ScriptableObject
{
    [Tooltip("�ړ����x")] public float MoveSpeed;
    [Tooltip("�U���J�n����")] public float AttackRange;
    [Tooltip("�U���N�[���^�C��")] public float AttackCoolTime;
    [Tooltip("�x����������")] public float AlertResetTime;
    [Tooltip("�_���[�W���󂯂Ȃ��p�x")] public float InvincibleAngle;
    [Tooltip("���񑬓x")] public float RotateSpeed;

    [Tooltip("�ǐՑΏۃ��X�g�̃��t���b�V�����[�g(�����قǒ�p�x)")] public ulong ListRefreshRate;
}
