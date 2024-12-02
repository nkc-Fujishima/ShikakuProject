using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effect/ShootEnemy", fileName = "ShootEnemyEffectObject")]
public class ShootEnemyEffectData : EnemyEffectDataBase
{
    [Tooltip("発射音SE")] public AudioClip ShootSE;
    [Tooltip("着弾時SE")] public AudioClip HitSE;
    [Tooltip("着弾時エフェクト")] public ParticleSystem HitEffect;
}
