using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("�������C���X�g�f��")] public DetectionEffectController DetectionEffect;

    [Tooltip("������SE")] public AudioClip DetectionSE;

    [Tooltip("�|���ꂽ��SE")]public AudioClip DownSE;
}
