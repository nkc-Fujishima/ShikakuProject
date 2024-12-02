using UnityEngine;

[CreateAssetMenu(fileName ="StageSelectData",menuName ="Stage/Data/StageSelect")]
public class StageSelectData : ScriptableObject
{
    [Tooltip("現在選択しているステージの値"),SerializeField] public int StageSelectNumber;

    [Tooltip("選択できるステージの最大数"),SerializeField] public int StageCountMax;
}
