using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class StageSelectImageController : MonoBehaviour
{
    [Header("オブジェクト設定"), SerializeField] GameObject stageSelectImage = null;

    [Header("数値設定"), Tooltip("ズーム間隔"), SerializeField] float zoomSpan;
    [Tooltip("ズーム最小値"), SerializeField] Vector3 zoomMinSize;
    [Tooltip("移動時間"), SerializeField] float moveTime;

    // Start is called before the first frame update
    void Start()
    {
        stageSelectImage.transform.DOScale(zoomMinSize, zoomSpan).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void MoveImagePosition(Image StageImage)
    {
        stageSelectImage.transform.DOMove(StageImage.rectTransform.position, moveTime);
    }

    public void ResetImagePosition(Vector3 position)
    {
        DOTween.Kill(this.transform);
        stageSelectImage.GetComponent<RectTransform>().anchoredPosition = position;
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
