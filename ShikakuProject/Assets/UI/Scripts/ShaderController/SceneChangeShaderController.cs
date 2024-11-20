using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeShaderController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("フェード用イメージ"), SerializeField] Image fadeImage;

    [Header("数値設定"), Tooltip("フェード速度"), SerializeField] float fadeSpeed;
    [Tooltip("画面分割数"), SerializeField] int divideScreen;

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
