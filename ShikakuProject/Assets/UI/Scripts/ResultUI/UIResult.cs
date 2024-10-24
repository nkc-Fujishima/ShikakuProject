using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIResult : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("リザルト背景オブジェクト"), SerializeField] Image resultBackImageObject;
    [Tooltip("リザルト背景"), SerializeField] Sprite[] resultBackImages;
    [Tooltip("リザルトテキスト"), SerializeField] Text resultTextObject;
    [Tooltip("リザルトテキスト文言"), SerializeField] string[] resultTexts;
    [Tooltip("リザルトテキストカラー"), SerializeField] Color32[] resultTextColors;
    [Tooltip("ステージ開始時に押すボタン情報オブジェクト"), SerializeField] GameObject buttonInfoObject;

    [Tooltip("背景のYサイズ"), SerializeField] float backImageYSize;
    [Tooltip("背景のYサイズ拡縮時間"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("文字のYサイズ"), SerializeField] float textYSize;
    [Tooltip("文字のYサイズ拡縮時間"), SerializeField] float textScaleChangeTime;
    [Tooltip("ボタン情報のYサイズ"), SerializeField] float infoYSize;
    [Tooltip("ボタン情報のYサイズ拡縮時間"), SerializeField] float infoScaleChangeTime;

    // ステージクリア時に表示するUI
    public async UniTask OpenGameClearUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;
        buttonInfoObject.SetActive(true);

        resultBackImageObject.sprite = resultBackImages[0];
        resultTextObject.text = resultTexts[0];
        resultTextObject.color = resultTextColors[0];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    // ステージ失敗時に表示するUI
    public async UniTask OpenGameFailedUI()
    {
        resultBackImageObject.enabled = true;
        resultTextObject.enabled = true;
        buttonInfoObject.SetActive(true);

        resultBackImageObject.sprite = resultBackImages[1];
        resultTextObject.text = resultTexts[1];
        resultTextObject.color = resultTextColors[1];

        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(textYSize, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);
    }

    public async UniTask CloseResultUI()
    {
        UniTask imageScaleTask = resultBackImageObject.rectTransform.DOScaleY(0, backImageScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask textScaleTask = resultTextObject.rectTransform.DOScaleY(0, textScaleChangeTime).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(imageScaleTask, textScaleTask);

        resultBackImageObject.enabled = false;
        resultTextObject.enabled = false;
        buttonInfoObject.SetActive(false);
    }
}
