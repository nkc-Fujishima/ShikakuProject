using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameStartShaderController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�t�F�[�h�p�C���[�W"), SerializeField] Image fadeImage;

    [Header("���l�ݒ�"), Tooltip("1�i�K�ڃt�F�[�h���x"), SerializeField] float fadeSpeedPhase1;
    [Tooltip("2�i�K�ڃt�F�[�h���x"), SerializeField] float fadeSpeedPhase2;
    [Tooltip("�t�F�[�h1�i�K�ڂɎ~�߂�A�j���[�V��������"), SerializeField] float fadeTaskPhase1;
    [Tooltip("�t�F�[�h2�i�K�ڂɎ~�߂�A�j���[�V��������"), SerializeField] float fadeTaskPhase2;
    [Tooltip("1�i�K�ڂ�2�i�K�ڂ̊Ԃ̎~�߂鎞��(�~���b)"), SerializeField] int fadeWaitTime;

    Material material = null;

    float countTime = 0;

    public void SetUp(Vector2 screenPos)
    {
        material = fadeImage.material;

        material.SetVector("_ExpandStartPos", new Vector4(screenPos.x / Screen.width, screenPos.y / Screen.height, 0, 0));

        // �t�F�[�h�p�V�F�[�_-�̃A�j���[�V�����Đ����Ԃ����Z�b�g
        material.SetFloat("_AnimationTime", 0);
    }

    public async UniTask FadeIn()
    {
        // �t�F�[�h1�i�K�ڂ̏���
        while (countTime < fadeTaskPhase1)
        {
            countTime += Time.deltaTime * fadeSpeedPhase1;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.Delay(fadeWaitTime);

        // �t�F�[�h2�i�K�ڂ̏���
        while (countTime < fadeTaskPhase2)
        {
            countTime += Time.deltaTime * fadeSpeedPhase2;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.CompletedTask;
    }
}
