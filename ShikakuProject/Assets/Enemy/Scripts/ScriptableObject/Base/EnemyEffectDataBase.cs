using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("発見時イラスト素材")] public DetectionEffectController detectionEffect;

    [Tooltip("発見時SE")] public AudioClip detectionSE;
}
