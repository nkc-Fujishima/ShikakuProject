using UnityEngine;

[CreateAssetMenu(menuName = "Stage/Gimmick/SandGimmickParameter", fileName = "SandGimmickParameter")]
public class SandGimmickParameter : ScriptableObject
{
    [Tooltip("����������")] public float SlidePower;
    [Tooltip("�ړ����x������������{��")] public float BrekePower;
}
