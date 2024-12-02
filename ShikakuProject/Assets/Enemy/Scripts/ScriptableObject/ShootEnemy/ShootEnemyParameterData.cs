using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Parameter/ShootEnemy", fileName = "ShootEnemyParameter")]
public class ShootEnemyParameterData : EnemyParameterDataBase
{
    [Tooltip("’e‚ÌˆÚ“®‘¬“x")] public float BulletSpeed;
    [Tooltip("’e‚Ì¶‘¶ŠÔ")] public float BulletLifeTime;
}
