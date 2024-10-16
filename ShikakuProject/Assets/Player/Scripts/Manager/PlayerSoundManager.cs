using UnityEngine;

[System.Serializable]
public class PlayerSoundManager
{
    [SerializeField]
    private AudioSource _audioSource;


    [SerializeField] private AudioClip _soundWalk;
    [SerializeField] private AudioClip _soundHit;
    [SerializeField] private AudioClip _soundSkillSelect;
    [SerializeField] private AudioClip _soundSkillCoolTimeMax;

    public void OnWalk() { StartSound(_soundWalk); }
    public void OnHit() { StartSound(_soundHit); }
    public void OnSkillSelect() { StartSound(_soundSkillSelect); }
    public void OnSkillCoolTimeMax() { StartSound(_soundSkillCoolTimeMax); }

    private void StartSound(in AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }


}