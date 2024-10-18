using UnityEngine;

[System.Serializable]
public class BulletSoundManager
{
    [SerializeField]
    private AudioSource _audioSource;


    [SerializeField] private AudioClip _soundSpawn;
    [SerializeField] private AudioClip _soundHit;


    public void OnSpawn() { StartSound(_soundSpawn); }
    public void OnHit() { StartSound(_soundHit); }

    private void StartSound(in AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }
}
