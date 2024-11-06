using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="StageSelect/Data/StageImage",fileName ="StageImageData")]
public class StageImageData : ScriptableObject
{
    [Tooltip("ステージイメージ画像")] public Sprite[] stageImages;
}
