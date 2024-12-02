using UnityEngine;

public static class DestroyAudioPlay 
{
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume)
    {
        // 音を鳴らす用オブジェクトの生成
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        // 音を鳴らす用オブジェクトにAudioSourceを追加、オプションを設定
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        // 鳴らし終えた場合、破棄
        Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}
