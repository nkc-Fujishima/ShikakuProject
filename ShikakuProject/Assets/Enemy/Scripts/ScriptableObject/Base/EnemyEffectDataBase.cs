using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEffectDataBase : ScriptableObject
{
    [Tooltip("発見時イラスト素材")] public DetectionEffectController DetectionEffect;

    [Tooltip("発見時SE")] public AudioClip DetectionSE;

    [Tooltip("倒された時SE")]public AudioClip DownSE;
}
