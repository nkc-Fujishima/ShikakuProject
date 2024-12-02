using UnityEngine;

public class SEController : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("SE素材"), SerializeField] AudioClip[] seClips;

    AudioSource audioSource = null;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 選択用のSEを鳴らします
    public void RingSelectSE()
    {
        audioSource.clip = seClips[0];
        audioSource.Play();
    }

    // ワールド決定用のSEを鳴らします
    public void RingWorldDicisionSE()
    {
        audioSource.clip = seClips[1];
        audioSource.Play();
    }

    // ステージ決定用のSEを鳴らします
    public void RingStageDicisionSE()
    {
        audioSource.clip = seClips[2];
        audioSource.Play();
    }

    // キャンセル用のSEを鳴らします
    public void RingCancelSE()
    {
        audioSource.clip = seClips[3];
        audioSource.Play();
    }
}
