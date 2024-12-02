using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SceneChangeShaderController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�t�F�[�h�p�C���[�W"), SerializeField] Image fadeImage;

    [Header("���l�ݒ�"), Tooltip("�t�F�[�h���x"), SerializeField] float fadeSpeed;
    [Tooltip("��ʕ�����"), SerializeField] int divideScreen;
    [Tooltip("�t�F�[�h���ԍő�l"), SerializeField] int fadeTimeMax;

    Material material = null;

    float countTime = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
    }

    /// <summary>
    /// �Z�b�g�A�b�v�p�֐��@��p������}�e���A���̎擾�Ɖ�ʕ����̒l���}�e���A���ɃZ�b�g���܂�
    /// </summary>
    public void SetUp()
    {
        material = fadeImage.material;

        int aspectRatio = divideScreen * Screen.height / Screen.width;
        material.SetVector("_DivideScreen", new Vector4(divideScreen, aspectRatio, 0, 0));
    }

    // �񓯊��Ńt�F�[�h�A�E�g���܂�
    public async UniTask FadeOut()
    {
        while (countTime < fadeTimeMax)
        {
            countTime += Time.deltaTime * fadeSpeed;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.CompletedTask;
    }

    // �񓯊��Ńt�F�[�h�C�����܂�
    public async UniTask FadeIn()
    {
        countTime = fadeTimeMax;

        while (countTime >= 0)
        {
            countTime -= Time.deltaTime * fadeSpeed;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.CompletedTask;
    }

    // ���݂̃A�j���[�V�������Ԃ�MAX�ɂ��܂�
    public void SetFadeValueMax()
    {
        material.SetFloat("_AnimationTime", fadeTimeMax);
    }

    // ���݂̃A�j���[�V�������Ԃ�0�ɂ��܂�
    public void SetFadeValueMin()
    {
        material.SetFloat("_AnimationTime", 0);
    }

}
