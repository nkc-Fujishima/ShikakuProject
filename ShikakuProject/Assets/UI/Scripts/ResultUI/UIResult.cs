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
    [Tooltip("ステージ終了時に押すボタン情報親オブジェクト"), SerializeField] GameObject buttonInfoObjectParent;
    [Tooltip("ステージ終了時のオブジェクト_1"), SerializeField] GameObject buttonInfoObject; 
    [Tooltip("ステージ終了時のインフォメーションテキスト"), SerializeField] Text nextActionText;

    [Header("数値設定")]
    [Tooltip("背景のYサイズ"), SerializeField] float backImageYSize;
    [Tooltip("背景のYサイズ拡縮時間"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("文字のYサイズ"), SerializeField] float textYSize;
    [Tooltip("文字のYサイズ拡縮時間"), SerializeField] float textScaleChangeTime;
    [Tooltip("ボタン情報のYサイズ"), SerializeField] float infoYSize;
    [Tooltip("ボタン情報のYサイズ拡縮時間"), SerializeField] float infoScaleChangeTime;

    [Header("文字設定"), Tooltip("次に行える行動を示すテキスト"), SerializeField] string[] nextActionStr;

    // ステージクリア時に表示するUI
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

    // ステージ失敗時に表示するUI
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

    // UIを閉じる
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
