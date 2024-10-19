using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITimeCountManager : MonoBehaviour
{
    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Image _timeImage;

    // 効果音関係
    [SerializeField]
    private TimeCountSoundManager _timeCountSoundManager;


    private const int WARNING_COUNT_START_SECOND = 5 + 1;


    private float _timeMax = 0;

    public void OnStart(float timeMax)
    {
        _timeText.text = timeMax.ToString();

        _timeImage.fillAmount = 0;

        _timeMax = timeMax;


        _timeCountSoundManager.OnStart(WARNING_COUNT_START_SECOND);
    }

    public void TimeCount(float time)
    {
        if (time < 0) time = 0;

        _timeText.text = time.ToString("F1");

        _timeImage.fillAmount = time / _timeMax;

        // 効果音関係
        _timeCountSoundManager.SetCount(time);

        // タイムアップに近づいてきたら色を変える
        if (time < WARNING_COUNT_START_SECOND)
            _timeText.DOColor(Color.red, 0.2f);
    }
}
