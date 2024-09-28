using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/ShootEnemy", fileName = "ShootEnemyParameter")]
public class ShootEnemyParameter : EnemyParameterBase
{
    [Tooltip("’e‚ÌˆÚ“®‘¬“x")] public float BulletSpeed;
    [Tooltip("’e‚Ì¶‘¶ŠÔ")] public float BulletLifeTime;
}
