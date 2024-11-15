using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITalkManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _imageTalk;

    [SerializeField]
    private Text _textTalk;

    [SerializeField]
    private Transform _transformTalk;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _talkUIOutlineThickness = 20;


    private readonly float _duration = 0.2f;

    private Vector2 _activePoint = Vector2.zero;

    private Vector2 _endPoint = new(0, 0.3f);

    public void OnStart(string text)
    {
        SetBackImageScale(in text);

        _textTalk.text = text;

        transform.rotation = Camera.main.transform.rotation;
    }

    // Œã‚ë‚Ì‰æ‘œ‚Ì‘å‚«‚³‚ð•¶Žš”‚É‡‚¤‚æ‚¤‚É’²ß
    private void SetBackImageScale(in string text)
    {
        Vector2 textImageLange = new();
        int paragraphCount = text.Split('\n').Length;
        textImageLange.y = paragraphCount * _textTalk.fontSize + _talkUIOutlineThickness * 2;

        int characterCount = CheckLengthLongestString(in text);
        textImageLange.x = characterCount * _textTalk.fontSize + _talkUIOutlineThickness * 2;

        _imageTalk.sizeDelta = textImageLange;
    }

    // Žw’è‚µ‚½•¶Žš—ñ‚ª‚¢‚­‚Â‚ ‚é‚©
    private int CheckLengthLongestString(in string text)
    {
        string[] paragraphs = text.Split('\n');
        int maxLength = 0;
        foreach (string paragraph in paragraphs)
        {
            if (paragraph.Length > maxLength)
            {
                maxLength = paragraph.Length;
            }
        }

        return maxLength;
    }


    public void ActiveTalk()
    {
        _canvasGroup.DOFade(1f, _duration);

        _transformTalk.DOLocalMove(_activePoint, _duration);
    }

    public void EndTalk()
    {
        _canvasGroup.DOFade(0, _duration);

        _transformTalk.DOLocalMove(_endPoint, _duration);
    }
}