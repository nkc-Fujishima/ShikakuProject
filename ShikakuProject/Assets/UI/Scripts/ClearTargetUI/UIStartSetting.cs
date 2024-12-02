using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UIStartSetting : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"),Tooltip("�N���A�ڕW�\���w�i"), SerializeField] Image clearTargetBackImage;
    [Tooltip("�N���A�ڕW�\���e�L�X�g"),SerializeField] Text clearTargetText;
    [Tooltip("�X�e�[�W�J�n���ɉ����{�^�����I�u�W�F�N�g"),SerializeField] GameObject buttonInfoObject;

    [Header("���l�ݒ�"), Tooltip("�N���A�ڕW�e�L�X�g���e"), SerializeField] string[] clearTargetTexts;
    [Tooltip("�w�i��Y�T�C�Y"), SerializeField] float backImageYSize;
    [Tooltip("�w�i��Y�T�C�Y�g�k����"), SerializeField] float backImageScaleChangeTime;
    [Tooltip("�{�^������Y�T�C�Y"), SerializeField] float infoYSize;
    [Tooltip("�{�^������Y�T�C�Y�g�k����"), SerializeField] float infoScaleChangeTime;
    [Tooltip("�e�L�X�g�̊J�n�z�u�ʒu"), SerializeField] Transform startTextXPosition;
    [Tooltip("�e�L�X�g�̏I���z�u�ʒu"), SerializeField] Transform endTextXPosition;
    [Tooltip("�e�L�X�g�̔z�u�܂ł̎���"), SerializeField] float textXPositionMoveTime;


    /// <summary>
    /// �X�^�[�g�p��UI��\�����I���܂Ń^�X�N��҂��܂�
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
    /// �X�^�[�g�p��UI����I���܂Ń^�X�N��҂��܂�
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