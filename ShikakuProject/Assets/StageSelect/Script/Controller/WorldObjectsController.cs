using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldObjectsController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("���[���h�C���[�W�I�u�W�F�N�g"), SerializeField]
    List<GameObject> worldImageObjects = new List<GameObject>();

    [Header("���l�ݒ�"), Tooltip("�ړ�����"), SerializeField] float moveTime;

    GameObject currentSelectObject = null;

    public void SetObjectPosition(int selectWorldObjectCount, Vector3 position)
    {
        if (selectWorldObjectCount < 0 || selectWorldObjectCount > worldImageObjects.Count - 1) return;

        worldImageObjects[selectWorldObjectCount].transform.DOMove(position, moveTime);
    }

    public void SetStageSelectStatePosition(Vector3 position)
    {
        currentSelectObject.transform.DOMove(position, moveTime);
    }

    public void SetCurrentSelectWorldObject(int selectWorldObjectCount)
    {
        currentSelectObject = worldImageObjects[selectWorldObjectCount];
    }
}
