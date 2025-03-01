using UnityEngine;

[CreateAssetMenu(menuName = "Stage/Gimmick/SandGimmickParameter", fileName = "SandGimmickParameter")]
public class SandGimmickParameter : ScriptableObject
{
    [Tooltip("押し流す力")] public float SlidePower;
    [Tooltip("移動速度を減少させる倍率")] public float BrekePower;
}
