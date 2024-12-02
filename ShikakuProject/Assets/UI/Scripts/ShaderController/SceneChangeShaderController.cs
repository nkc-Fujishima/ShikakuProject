using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SceneChangeShaderController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("フェード用イメージ"), SerializeField] Image fadeImage;

    [Header("数値設定"), Tooltip("フェード速度"), SerializeField] float fadeSpeed;
    [Tooltip("画面分割数"), SerializeField] int divideScreen;
    [Tooltip("フェード時間最大値"), SerializeField] int fadeTimeMax;

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
    /// セットアップ用関数　作用させるマテリアルの取得と画面分割の値をマテリアルにセットします
    /// </summary>
    public void SetUp()
    {
        material = fadeImage.material;

        int aspectRatio = divideScreen * Screen.height / Screen.width;
        material.SetVector("_DivideScreen", new Vector4(divideScreen, aspectRatio, 0, 0));
    }

    // 非同期でフェードアウトします
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

    // 非同期でフェードインします
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

    // 現在のアニメーション時間をMAXにします
    public void SetFadeValueMax()
    {
        material.SetFloat("_AnimationTime", fadeTimeMax);
    }

    // 現在のアニメーション時間を0にします
    public void SetFadeValueMin()
    {
        material.SetFloat("_AnimationTime", 0);
    }

}
