using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIResult : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("���U���g�w�i�I�u�W�F�N�g"), SerializeField]
    Image resultBackImageObject;
    [Tooltip("���U���g�w�i"), SerializeField]
    Sprite[] resultBackImages;
    [Tooltip("���U���g�e�L�X�g"), SerializeField]
    Text resultTextObject;
    [Tooltip("���U���g�e�L�X�g����"), SerializeField]
    string[] resultTexts;
    [Tooltip("���U���g�e�L�X�g�J���["), SerializeField]
    Color32[] resultTextColors;

    [Tooltip("�w�i��Y�T�C�Y"), SerializeField] float backImageYSize;
    [Tooltip("�w�i��Y�T�C�Y�g�k����"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("������Y�T�C�Y"), SerializeField] float textYSize;
    [Tooltip("������Y�T�C�Y�g�k����"), SerializeField] float textScaleChangeTime;

    public async UniTask OpenGameClearUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;

        resultBackImageObject.sprite = resultBackImages[0];
        resultTextObject.text = resultTexts[0];
        resultTextObject.color = resultTextColors[0];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    public async UniTask OpenGameFailedUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;

        resultBackImageObject.sprite = resultBackImages[1];
        resultTextObject.text = resultTexts[1];
        resultTextObject.color = resultTextColors[1];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    public async UniTask CloseResultUI()
    {
        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(0, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(0, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);

        resultBackImageObject.enabled = false;
        resultTextObject.enabled = false;
    }
}
