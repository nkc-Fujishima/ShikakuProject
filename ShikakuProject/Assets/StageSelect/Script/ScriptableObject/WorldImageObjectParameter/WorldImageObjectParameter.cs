using UnityEngine;

[CreateAssetMenu(menuName ="StageSelect/Parameter/StageSelectParameter",fileName ="StageSelectParameter")]
public class WorldImageObjectParameter : ScriptableObject
{
    [Tooltip("ステージイメージ回転速度")] public float rotateSpeed;
}
