using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameStartShaderController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("フェード用イメージ"), SerializeField] Image fadeImage;

    [Header("数値設定"), Tooltip("1段階目フェード速度"), SerializeField] float fadeSpeedPhase1;
    [Tooltip("2段階目フェード速度"), SerializeField] float fadeSpeedPhase2;
    [Tooltip("フェード1段階目に止めるアニメーション時間"), SerializeField] float fadeTaskPhase1;
    [Tooltip("フェード2段階目に止めるアニメーション時間"), SerializeField] float fadeTaskPhase2;
    [Tooltip("1段階目と2段階目の間の止める時間(ミリ秒)"), SerializeField] int fadeWaitTime;

    Material material = null;

    float countTime = 0;

    public void SetUp(Vector2 screenPos)
    {
        material = fadeImage.material;

        material.SetVector("_ExpandStartPos", new Vector4(screenPos.x / Screen.width, screenPos.y / Screen.height, 0, 0));

        // フェード用シェーダ-のアニメーション再生時間をリセット
        material.SetFloat("_AnimationTime", 0);
    }

    public async UniTask FadeIn()
    {
        // フェード1段階目の処理
        while (countTime < fadeTaskPhase1)
        {
            countTime += Time.deltaTime * fadeSpeedPhase1;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.Delay(fadeWaitTime);

        // フェード2段階目の処理
        while (countTime < fadeTaskPhase2)
        {
            countTime += Time.deltaTime * fadeSpeedPhase2;

            material.SetFloat("_AnimationTime", countTime);

            await UniTask.Yield();
        }

        await UniTask.CompletedTask;
    }
}
