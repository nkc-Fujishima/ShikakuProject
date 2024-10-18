using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("�������G�t�F�N�g")] public DetectionEffectController DetectionEffect;

    [Tooltip("������SE")] public AudioClip DetectionSE;

    [Tooltip("�|���ꂽ�Ƃ��G�t�F�N�g")] public ParticleSystem DownEffect;

    [Tooltip("�|���ꂽ��SE")]public AudioClip DownSE;
}
