using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/ShootEnemy", fileName = "ShootEnemyParameter")]
public class ShootEnemyParameterData : EnemyParameterDataBase
{
    [Tooltip("弾の移動速度")] public float BulletSpeed;
    [Tooltip("弾の生存時間")] public float BulletLifeTime;
}
