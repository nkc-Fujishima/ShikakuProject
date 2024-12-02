using UnityEngine;
using DG.Tweening;

public class StageImagesController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�X�e�[�W�C���[�W�I�u�W�F�N�g"), SerializeField] GameObject stageImagesObject;

    [Header("���l�ݒ�"), Tooltip("�ړ�����"), SerializeField] float moveTime;
    [Tooltip("�ړ��ʒu"), SerializeField] Vector3[] movePos;

    RectTransform rectTransform = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// �����̒l�̗v�f�ɂ���ړ��ꏊ�ɃZ���N�g�C���[�W�摜���ړ������܂�
    /// </summary>
    /// <param name="stage"></param>
    public void SetPosition(int stage)
    {
        rectTransform.DOAnchorPos(movePos[stage], moveTime);
    }
}
