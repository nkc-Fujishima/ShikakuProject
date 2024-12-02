using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectImageController : MonoBehaviour
{
    [Header("オブジェクト設定"), SerializeField] GameObject stageSelectImage = null;

    [Header("数値設定"), Tooltip("ズーム間隔"), SerializeField] float zoomSpan;
    [Tooltip("ズーム最小値"), SerializeField] Vector3 zoomMinSize;
    [Tooltip("初期位置"), SerializeField] Vector3 initialPos;
    [Tooltip("移動時間"), SerializeField] float moveTime;
    [Tooltip("セレクトイメージ画像Z位置オフセット"), SerializeField] float offsetPosZ;

    // Start is called before the first frame update
    void Start()
    {
        stageSelectImage.transform.DOScale(zoomMinSize, zoomSpan).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void MoveImagePosition(Image StageImage)
    {
        stageSelectImage.transform.DOMove(new Vector3(StageImage.rectTransform.position.x, StageImage.rectTransform.position.y, StageImage.rectTransform.position.z - offsetPosZ), moveTime);
    }

    public void ResetImagePosition()
    {
        DOTween.Kill(this.transform);
        stageSelectImage.GetComponent<RectTransform>().anchoredPosition = initialPos;
    }

    public void RestartZoom()
    {
        stageSelectImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        stageSelectImage.transform.DOScale(zoomMinSize, zoomSpan).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void Enable()
    {
        stageSelectImage.SetActive(true);
    }

    public void Disable()
    {
        stageSelectImage.SetActive(false);
    }

}
