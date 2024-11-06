using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldObjectsController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ワールドイメージオブジェクト"), SerializeField]
    List<GameObject> worldImageObjects = new List<GameObject>();

    [Header("数値設定"), Tooltip("移動時間"), SerializeField] float moveTime;

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
