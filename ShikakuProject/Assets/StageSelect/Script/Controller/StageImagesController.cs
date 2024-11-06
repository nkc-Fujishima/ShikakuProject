using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageImagesController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�X�e�[�W�C���[�W�I�u�W�F�N�g"), SerializeField] GameObject stageImagesObject;

    [Header("���l�ݒ�"), Tooltip("�ړ�����"), SerializeField] float moveTime;

    RectTransform rectTransform = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition(Vector3 position)
    {
        rectTransform.DOAnchorPos(position, moveTime);
    }
}
