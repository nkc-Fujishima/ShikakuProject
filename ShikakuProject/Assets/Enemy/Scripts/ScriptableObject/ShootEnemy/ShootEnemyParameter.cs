using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/ShootEnemy", fileName = "ShootEnemyParameter")]
public class ShootEnemyParameter : EnemyParameterBase
{
    [Tooltip("弾の移動速度")] public float BulletSpeed;
    [Tooltip("弾の生存時間")] public float BulletLifeTime;
}
