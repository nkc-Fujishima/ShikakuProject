using UnityEngine;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("�������G�t�F�N�g")] public DetectionEffectController DetectionEffect;

    [Tooltip("������SE")] public AudioClip DetectionSE;

    [Tooltip("�|���ꂽ�Ƃ��G�t�F�N�g")] public ParticleSystem DownEffect;

    [Tooltip("�|���ꂽ��SE")]public AudioClip DownSE;

    [Tooltip("������Ƃ��G�t�F�N�g")] public ParticleSystem DestroyEffect;

    [Tooltip("������Ƃ�SE")] public AudioClip DestroySE;
}
