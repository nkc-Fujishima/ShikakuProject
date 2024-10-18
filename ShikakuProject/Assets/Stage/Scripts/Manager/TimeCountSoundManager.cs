using UnityEngine;

[System.Serializable]
public class TimeCountSoundManager
{
    [Header("Œø‰Ê‰¹ŠÖŒW")]
    [SerializeField]
    private AudioClip _soundTimeCount;

    [SerializeField]
    private AudioSource _audioSource;

    private float _countDownSecond = 0;


    public void OnStart(float countMax)
    {
        _countDownSecond = countMax;
    }


    public void SetCount(float value)
    {
        if (value <= _countDownSecond)
        {
            --_countDownSecond;
            CountTimer();
        }

        if (_countDownSecond == 0)
            _countDownSecond = -10;
    }

    public void CountTimer()
    {
        _audioSource.PlayOneShot(_soundTimeCount);
    }
}
