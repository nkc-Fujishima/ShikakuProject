using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

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

    public void SetUp()
    {
        material = fadeImage.material;

        int aspectRatio = divideScreen * Screen.height / Screen.width;
        material.SetVector("_DivideScreen", new Vector4(divideScreen, aspectRatio, 0, 0));
    }

    public async UniTask FadeIn()
    {
        while (countTime < fadeTimeMax)
        {
            countTime += Time.deltaTime * fadeSpeed;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.CompletedTask;
    }
    public async UniTask FadeOut()
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

    public void SetFadeValueMax()
    {
        material.SetFloat("_AnimationTime", fadeTimeMax);
    }

    public void SetFadeValueMin()
    {
        material.SetFloat("_AnimationTime", 0);
    }

}
