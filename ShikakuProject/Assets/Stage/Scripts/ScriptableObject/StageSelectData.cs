using UnityEngine;

[CreateAssetMenu(fileName ="StageSelectData",menuName ="Stage/Data/StageSelect")]
public class StageSelectData : ScriptableObject
{
    [Tooltip("���ݑI�����Ă���X�e�[�W�̒l"),SerializeField] public int StageSelectNumber;

    [Tooltip("�I���ł���X�e�[�W�̍ő吔"),SerializeField] public int StageCountMax;
}
