using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeShaderController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�t�F�[�h�p�C���[�W"), SerializeField] Image fadeImage;

    [Header("���l�ݒ�"), Tooltip("�t�F�[�h���x"), SerializeField] float fadeSpeed;
    [Tooltip("��ʕ�����"), SerializeField] int divideScreen;

    Material material = null;

    float countTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        material = fadeImage.material;

        int aspectRatio = divideScreen * Screen.height / Screen.width;
        material.SetVector("_DivideScreen", new Vector4(divideScreen, aspectRatio, 0, 0));
    }

    void Update()
    {
        countTime += Time.deltaTime * fadeSpeed;

        material.SetFloat("_AnimationTime", countTime);
    }

}
