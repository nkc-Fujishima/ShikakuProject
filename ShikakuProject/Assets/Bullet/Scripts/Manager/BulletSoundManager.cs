using UnityEngine;

[System.Serializable]
public class BulletSoundManager
{
    [SerializeField]
    private AudioSource _audioSource;


    [SerializeField] private AudioClip _soundSpawn;
    [SerializeField] private AudioClip _soundHit;
    [SerializeField] private AudioClip _soundDeath;


    public void OnSpawn() { StartSound(_soundSpawn); }
    public void OnHit() { StartSound(_soundHit); }
    public void OnDeath(in Vector3 point) { StartDestroySound(_soundDeath,point); }

    private void StartSound(in AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }

    private void StartDestroySound(in AudioClip sound, in Vector3 point)
    {
        DestroyAudioPlay.PlayClipAtPoint(sound, point, 1f);
    }
}
