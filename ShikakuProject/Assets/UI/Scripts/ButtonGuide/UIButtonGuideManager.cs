using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class UIButtonGuideManager : MonoBehaviour
{

    [SerializeField]
    private Transform _guideImage;

    [SerializeField]
    private Transform _activePoint;

    [SerializeField]
    private Transform _notActivePoint;

    private bool _isFirst = true;

    public void SetActiveUI(bool isActive)
    {
        Transform standardPoint = isActive ? _activePoint : _notActivePoint;

        if(_isFirst)
        {
            Vector3 newPoint = transform.position;
            newPoint.y= _notActivePoint.transform.position.y;
            _guideImage.position = newPoint;
            _isFirst = false;
            return;
        }
        _guideImage.DOMoveY(standardPoint.position.y, 0.5f);

    }
}
