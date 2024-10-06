using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UIClearTarget : MonoBehaviour
{
    [Header("オブジェクト設定"), SerializeField] Image clearTargetBackImage;
    [SerializeField] Text clearTargetText;
    [SerializeField] GameObject buttonInfoObject;

    [Header("数値設定"), Tooltip("クリア目標テキスト"), SerializeField] string[] clearTargetTexts;
    [Tooltip("背景のYサイズ"), SerializeField] float backImageYSize;
    [Tooltip("背景のYサイズ拡縮時間"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("ボタン情報のYサイズ"), SerializeField] float infoYSize;
    [Tooltip("ボタン情報のYサイズ拡縮時間"), SerializeField] float infoScaleChangeTime;
    [Tooltip("テキストの開始配置位置"), SerializeField] float startTextXPosition;
    [Tooltip("テキストの終了配置位置"), SerializeField] float endTextXPosition;
    [Tooltip("テキストの配置までの時間"), SerializeField] float textXPositionMoveTime;


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
        UniTask moveTask = clearTargetText.rectTransform.DOMoveX(startTextXPosition, textXPositionMoveTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(infoYSize, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(scaleTask, moveTask, infoTask);
    }

    public async UniTask CloseGameStartUI()
    {
        UniTask scaleTask = clearTargetBackImage.rectTransform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask moveTask = clearTargetText.rectTransform.DOMoveX(endTextXPosition, textXPositionMoveTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();
        UniTask infoTask = buttonInfoObject.transform.DOScaleY(0, backImageScaleChangeTime).SetEase(Ease.OutCubic).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(scaleTask, moveTask, infoTask);

        clearTargetBackImage.enabled = false;
        clearTargetText.enabled = false;
        buttonInfoObject.SetActive(false);
    }
}