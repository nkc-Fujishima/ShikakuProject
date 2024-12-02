using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effect/ShootEnemy", fileName = "ShootEnemyEffectObject")]
public class ShootEnemyEffectData : EnemyEffectDataBase
{
    [Tooltip("���ˉ�SE")] public AudioClip ShootSE;
    [Tooltip("���e��SE")] public AudioClip HitSE;
    [Tooltip("���e���G�t�F�N�g")] public ParticleSystem HitEffect;
}
