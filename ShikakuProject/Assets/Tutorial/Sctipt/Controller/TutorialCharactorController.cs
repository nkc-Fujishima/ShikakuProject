using UnityEngine;

public class TutorialCharactorController : MonoBehaviour, ITalkable
{
    [SerializeField]
    private UITalkManager _talkManager;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField]
    private Rigidbody _rigidbodyChara;

    [SerializeField]
    private float _sensingDistance = 10f;


    private string text;

    private Transform _transformChara;

    private bool _isTalking = false;


    private void Start()
    {
        _talkManager.OnStart(text);

        _transformChara = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if ((_transformChara.position - transform.position).magnitude > _sensingDistance)
        {
            if (!_isTalking) return;

            OnEnd();
            _isTalking = false;

            return;
        }

        if (_isTalking) return;

        OnActive();
        _isTalking = true;
    }

    private void OnActive()
    {
        if (_audioClip && _audioSource)
            _audioSource.PlayOneShot(_audioClip);

        if (_animator)
            _animator.SetTrigger("jump");

        _rigidbodyChara.AddForce(0, 200, 0);

        _talkManager.ActiveTalk();
    }

    private void OnEnd()
    {
        _talkManager.EndTalk();
    }


    // -------------------------------------------------------
    // ITalkable
    public string TalkText { set { text = value; } }
}