using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParameterBase : ScriptableObject
{
    [Tooltip("�ړ����x")] public float MoveSpeed;
    [Tooltip("�U���J�n����")] public float AttackRange;
    [Tooltip("�x����������")] public float alertResetTime;

    [Tooltip("�ǐՑΏۃ��X�g�̃��t���b�V�����[�g(�����قǒ�p�x)")] public ulong ListRefreshRate;
}
