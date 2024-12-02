using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectCursorController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("左側カーソルイメージ"), SerializeField] Image leftCursorImage;
    [Tooltip("右側カーソルイメージ"), SerializeField] Image rightCursorImage;

    [Header("数値設定"), Tooltip("移動時間"), SerializeField] float moveTime;
    [Tooltip("外側への移動倍率"), SerializeField] float moveOutSideRatio;

    // Start is called before the first frame update
    void Start()
    {
        leftCursorImage.rectTransform.DOMoveX(leftCursorImage.transform.position.x * moveOutSideRatio, moveTime).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
        rightCursorImage.rectTransform.DOMoveX(rightCursorImage.transform.position.x * moveOutSideRatio, moveTime).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    }

    public void HideLeftCursorImage()
    {
        leftCursorImage.gameObject.SetActive(false);

    }

    public void HideRightCursorImage()
    {
        rightCursorImage.gameObject.SetActive(false);
    }

    public void HideCursorImage()
    {
        leftCursorImage.gameObject.SetActive(false);
        rightCursorImage.gameObject.SetActive(false);

    }

    public void VisibleCursorImage()
    {
        leftCursorImage.gameObject.SetActive(true);
        rightCursorImage.gameObject.SetActive(true);
    }
}
