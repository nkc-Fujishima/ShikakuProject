using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("�������C���X�g�f��")] public DetectionEffectController detectionEffect;

    [Tooltip("������SE")] public AudioClip detectionSE;
}
