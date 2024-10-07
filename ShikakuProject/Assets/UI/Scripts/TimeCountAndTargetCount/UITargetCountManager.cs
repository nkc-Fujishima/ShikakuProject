using UnityEngine;
using UnityEngine.UI;

public class UITargetCountManager : MonoBehaviour
{
    [SerializeField]
    private Text _targetText;

    private int _targetMax = 0;

    public void OnStart(int targetMax)
    {
        _targetMax = targetMax;

        TargetCount(targetMax);
    }

    public void TargetCount(int targetRemaining)
    {
        _targetText.text = targetRemaining + "/" + _targetMax;
    }
}
