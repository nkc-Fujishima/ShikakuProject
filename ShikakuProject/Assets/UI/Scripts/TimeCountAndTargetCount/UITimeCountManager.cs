using UnityEngine;
using UnityEngine.UI;

public class UITimeCountManager : MonoBehaviour
{
    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Image _timeImage;

    private float _timeMax = 0;

    public void OnStart(float timeMax)
    {
        _timeText.text = timeMax.ToString();

        _timeImage.fillAmount = 0;

        _timeMax = timeMax;
    }

    public void TimeCount(float time)
    {
        if (time < 0) time = 0;

        _timeText.text = time.ToString("F1");

        _timeImage.fillAmount = time / _timeMax;
    }
}
