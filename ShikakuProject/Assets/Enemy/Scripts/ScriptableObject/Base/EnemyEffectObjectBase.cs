using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectObjectBase : ScriptableObject
{
    [Tooltip("�������C���X�g�f��")] public ParticleSystem detectionEffect;

    [Tooltip("������SE")] public AudioSource detectionSE;
}
