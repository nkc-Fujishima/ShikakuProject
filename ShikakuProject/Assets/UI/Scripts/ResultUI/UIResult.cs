using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIResult : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("���U���g�w�i�I�u�W�F�N�g"), SerializeField] Image resultBackImageObject;
    [Tooltip("���U���g�w�i"), SerializeField] Sprite[] resultBackImages;
    [Tooltip("���U���g�e�L�X�g"), SerializeField] Text resultTextObject;
    [Tooltip("���U���g�e�L�X�g����"), SerializeField] string[] resultTexts;
    [Tooltip("���U���g�e�L�X�g�J���["), SerializeField] Color32[] resultTextColors;
    [Tooltip("�X�e�[�W�I�����ɉ����{�^�����e�I�u�W�F�N�g"), SerializeField] GameObject buttonInfoObjectParent;
    [Tooltip("�X�e�[�W�I�����̃I�u�W�F�N�g_1"), SerializeField] GameObject buttonInfoObject; 
    [Tooltip("�X�e�[�W�I�����̃C���t�H���[�V�����e�L�X�g"), SerializeField] Text nextActionText;

    [Header("���l�ݒ�")]
    [Tooltip("�w�i��Y�T�C�Y"), SerializeField] float backImageYSize;
    [Tooltip("�w�i��Y�T�C�Y�g�k����"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("������Y�T�C�Y"), SerializeField] float textYSize;
    [Tooltip("������Y�T�C�Y�g�k����"), SerializeField] float textScaleChangeTime;
    [Tooltip("�{�^������Y�T�C�Y"), SerializeField] float infoYSize;
    [Tooltip("�{�^������Y�T�C�Y�g�k����"), SerializeField] float infoScaleChangeTime;

    [Header("�����ݒ�"), Tooltip("���ɍs����s���������e�L�X�g"), SerializeField] string[] nextActionStr;

    // �X�e�[�W�N���A���ɕ\������UI
    public async UniTask OpenGameClearUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;
        buttonInfoObjectParent.SetActive(true);

        resultBackImageObject.sprite = resultBackImages[0];
        resultTextObject.text = resultTexts[0];
        resultTextObject.color = resultTextColors[0];

        nextActionText.text = nextActionStr[0];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObjectParent.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    // �X�e�[�W���s���ɕ\������UI
    public async UniTask OpenGameFailedUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;
        buttonInfoObjectParent.SetActive(true);

        resultBackImageObject.sprite = resultBackImages[1];
        resultTextObject.text = resultTexts[1];
        resultTextObject.color = resultTextColors[1];

        nextActionText.text = nextActionStr[1];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObjectParent.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    // UI�����
    public async UniTask CloseResultUI()
    {
        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(0, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(0, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObjectParent.transform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);

        resultBackImageObject.enabled = false;
        resultTextObject.enabled = false;
        buttonInfoObjectParent.SetActive(false);
    }

    public void HideNextAction1()
    {
        buttonInfoObject.SetActive(false);
    }
}
