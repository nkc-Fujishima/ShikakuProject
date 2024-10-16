using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/ShootEnemy", fileName = "ShootEnemyParameter")]
public class ShootEnemyParameterData : EnemyParameterDataBase
{
    [Tooltip("�e�̈ړ����x")] public float BulletSpeed;
    [Tooltip("�e�̐�������")] public float BulletLifeTime;
}
