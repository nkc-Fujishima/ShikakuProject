using UnityEngine;
using DG.Tweening;

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

    /// <summary>
    /// 引数の値の要素にある移動場所にセレクトイメージ画像を移動させます
    /// </summary>
    /// <param name="stage"></param>
    public void SetPosition(int stage)
    {
        rectTransform.DOAnchorPos(movePos[stage], moveTime);
    }
}
