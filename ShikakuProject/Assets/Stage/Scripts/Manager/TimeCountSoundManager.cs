using UnityEngine;

[System.Serializable]
public class TimeCountSoundManager
{
    [Header("Œø‰Ê‰¹ŠÖŒW")]
    [SerializeField]
    private AudioClip _soundTimeCount;

    [SerializeField]
    private AudioSource _audioSource;


    private const int COUNT_START_SECOND = 5;

    private int _countDownSecond = COUNT_START_SECOND + 1;


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
