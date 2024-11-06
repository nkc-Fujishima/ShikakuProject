using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldObjectsController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ワールドイメージオブジェクト"), SerializeField]
    List<GameObject> worldImageObjects = new List<GameObject>();

    [Header("数値設定"), Tooltip("移動時間"), SerializeField] float moveTime;
    [Tooltip("ワールドセレクト時移動位置"), SerializeField] Vector3[] movePos;

    [Tooltip("ステージセレクトとワールドセレクト切り替え時の位置"), SerializeField]
    Vector3[] waitPos;

    GameObject currentSelectObject = null;


    // ワールドセレクトでのワールド切り替え時--------------------------------------------------------------------------
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
