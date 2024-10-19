using UnityEngine;
using UnityEngine.UI;

public class UITargetCountManager : MonoBehaviour
{
    [SerializeField]
    private Text _textTarget;

    [SerializeField]
    private Text _textTargetMax;
    

    public void OnStart(int targetMax)
    {
        _textTargetMax.text = targetMax.ToString();

        TargetCount(targetMax);

    }

    public void TargetCount(int targetRemaining)
    {
        _textTarget.text = targetRemaining.ToString();
    }
}
