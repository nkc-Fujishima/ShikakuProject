using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectCursorController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�����J�[�\���C���[�W"), SerializeField] Image leftCursorImage;
    [Tooltip("�E���J�[�\���C���[�W"), SerializeField] Image rightCursorImage;

    [Header("���l�ݒ�"), Tooltip("�ړ�����"), SerializeField] float moveTime;
    [Tooltip("�O���ւ̈ړ��{��"), SerializeField] float moveOutSideRatio;

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
