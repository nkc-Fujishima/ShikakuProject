using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectImageController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), SerializeField] GameObject stageSelectImage = null;

    [Header("���l�ݒ�"), Tooltip("�Y�[���Ԋu"), SerializeField] float zoomSpan;
    [Tooltip("�Y�[���ŏ��l"), SerializeField] Vector3 zoomMinSize;
    [Tooltip("�����ʒu"), SerializeField] Vector3 initialPos;
    [Tooltip("�ړ�����"), SerializeField] float moveTime;
    [Tooltip("�Z���N�g�C���[�W�摜Z�ʒu�I�t�Z�b�g"), SerializeField] float offsetPosZ;

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
