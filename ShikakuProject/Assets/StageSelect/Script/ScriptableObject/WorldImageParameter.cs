using UnityEngine;

[CreateAssetMenu(menuName ="StageSelect/Parameter/StageSelectParameter",fileName ="StageSelectParameter")]
public class WorldImageParameter : ScriptableObject
{
    [Tooltip("ステージイメージ回転速度")] public float rotateSpeed;
}
