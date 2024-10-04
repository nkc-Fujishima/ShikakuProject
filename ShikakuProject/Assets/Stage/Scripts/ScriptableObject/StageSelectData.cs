using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StageSelectData",menuName ="Stage/Data/StageSelect")]
public class StageSelectData : ScriptableObject
{
    [SerializeField] public int StageSelectNumber;
}
