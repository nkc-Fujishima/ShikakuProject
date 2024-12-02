using UnityEngine;

public static class DestroyAudioPlay 
{
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume)
    {
        // ����炷�p�I�u�W�F�N�g�̐���
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        // ����炷�p�I�u�W�F�N�g��AudioSource��ǉ��A�I�v�V������ݒ�
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        // �炵�I�����ꍇ�A�j��
        Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}
