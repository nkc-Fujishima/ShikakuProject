using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UIStartSetting : MonoBehaviour
{
    [Header("オブジェクト設定"),Tooltip("クリア目標表示背景"), SerializeField] Image clearTargetBackImage;
    [Tooltip("クリア目標表示テキスト"),SerializeField] Text clearTargetText;
    [Tooltip("ステージ開始時に押すボタン情報オブジェクト"),SerializeField] GameObject buttonInfoObject;

    [Header("数値設定"), Tooltip("クリア目標テキスト内容"), SerializeField] string[] clearTargetTexts;
    [Tooltip("背景のYサイズ"), SerializeField] float backImageYSize;
    [Tooltip("背景のYサイズ拡縮時間"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("ボタン情報のYサイズ"), SerializeField] float infoYSize;
    [Tooltip("ボタン情報のYサイズ拡縮時間"), SerializeField] float infoScaleChangeTime;
    [Tooltip("テキストの開始配置位置"), SerializeField] Transform startTextXPosition;
    [Tooltip("テキストの終了配置位置"), SerializeField] Transform endTextXPosition;
    [Tooltip("テキストの配置までの時間"), SerializeField] float textXPositionMoveTime;


    /// <summary>
    /// スタート用のUIを表示し終わるまでタスクを待ちます
    /// </summary>
    /// <param name="gameType"></param>
    /// <returns></returns>
    public async UniTask OpenGameStartUI(int gameType)
    {

        switch (gameType)
        {
            case 0:
                clearTargetText.text = clearTargetTexts[0];
                break;

            case 1:
                clearTargetText.text = clearTargetTexts[1];
                break;

            case 2:
                clearTargetText.text = clearTargetTexts[2];
                break;
        }

        clearTargetBackImage.enabled = true;
        clearTargetText.enabled = true;
        buttonInfoObject.SetActive(true);

        UniTask scaleTask = clearTargetBackImage.rectTransform.DOScaleY(backImageYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask moveTask = clearTargetText.rectTransform.DOMoveX(startTextXPosition.position.x, textXPositionMoveTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(scaleTask, moveTask, infoTask);
    }

    /// <summary>
    /// スタート用のUIを閉じ終わるまでタスクを待ちます
    /// </summary>
    /// <returns></returns>
    public async UniTask CloseGameStartUI()
    {
        UniTask scaleTask = clearTargetBackImage.rectTransform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask moveTask = clearTargetText.rectTransform.DOMoveX(endTextXPosition.position.x, textXPositionMoveTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(scaleTask, moveTask, infoTask);

        clearTargetBackImage.enabled = false;
        clearTargetText.enabled = false;
        buttonInfoObject.SetActive(false);
    }
}