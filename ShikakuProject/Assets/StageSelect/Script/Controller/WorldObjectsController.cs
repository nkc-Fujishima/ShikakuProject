using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldObjectsController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("���[���h�C���[�W�I�u�W�F�N�g"), SerializeField]
    List<GameObject> worldImageObjects = new List<GameObject>();

    [Header("���l�ݒ�"), Tooltip("�ړ�����"), SerializeField] float moveTime;
    [Tooltip("���[���h�Z���N�g���ړ��ʒu"), SerializeField] Vector3[] movePos;

    [Tooltip("�X�e�[�W�Z���N�g�ƃ��[���h�Z���N�g�؂�ւ����̈ʒu"), SerializeField]
    Vector3[] waitPos;

    GameObject currentSelectObject = null;


    // ���[���h�Z���N�g�ł̃��[���h�؂�ւ���--------------------------------------------------------------------------
    public void SetMainPosition(int selectWorldObjectCount)
    {
        if (selectWorldObjectCount < 0 || selectWorldObjectCount > worldImageObjects.Count - 1) return;

        worldImageObjects[selectWorldObjectCount].transform.DOMove(movePos[0], moveTime);
    }
    public void SetPrevPosition(int selectWorldObjectCount)
    {
        if (selectWorldObjectCount < 0 || selectWorldObjectCount > worldImageObjects.Count - 1) return;

        worldImageObjects[selectWorldObjectCount].transform.DOMove(movePos[1], moveTime);
    }
    public void SetMainNextPosition(int selectWorldObjectCount)
    {
        if (selectWorldObjectCount < 0 || selectWorldObjectCount > worldImageObjects.Count - 1) return;

        worldImageObjects[selectWorldObjectCount].transform.DOMove(movePos[2], moveTime);
    }
    //-----------------------------------------------------------------------------------------------------------------

    public void SetWorldSelectStatePosition()
    {
        currentSelectObject.transform.DOMove(waitPos[0], moveTime);
    }

    public void SetStageSelectStatePosition()
    {
        currentSelectObject.transform.DOMove(waitPos[1], moveTime);
    }

    public void SetCurrentSelectWorldObject(int selectWorldObjectCount)
    {
        currentSelectObject = worldImageObjects[selectWorldObjectCount];
    }
}
