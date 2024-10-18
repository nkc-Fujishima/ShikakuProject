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
    [SerializeField] private AudioClip _soundEvasion;

    public void OnWalk() { StartSound(_soundWalk); }
    public void OnHit() { StartSound(_soundHit); }
    public void OnSkillSelect() { StartSound(_soundSkillSelect); }
    public void OnSkillCoolTimeMax() { StartSound(_soundSkillCoolTimeMax); }
    public void OnEvasion() { StartSound(_soundEvasion); }

    private void StartSound(in AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }


}