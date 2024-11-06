using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageImagesController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ステージイメージオブジェクト"), SerializeField] GameObject stageImagesObject;

    [Header("数値設定"), Tooltip("移動時間"), SerializeField] float moveTime;
    [Tooltip("移動位置"), SerializeField] Vector3[] movePos;

    RectTransform rectTransform = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition(int state)
    {
        rectTransform.DOAnchorPos(movePos[state], moveTime);
    }
}
