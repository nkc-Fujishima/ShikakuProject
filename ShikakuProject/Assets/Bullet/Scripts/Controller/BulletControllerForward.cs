using UnityEngine;

// �^�������i�ޒe
public class BulletControllerForward : BulletControllerBase
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField]
    Animator _animator;

    private new void Start()
    {
        base.Start();

        _animator.SetInteger("Walk", 1);
    }

    private new void Update()
    {
        base.Update();
        OnMove();
    }

    //----------------------------------------------------------------------------------
    // �ړ��֐�
    protected override void OnMove()
    {
        if (IsStop) return;

        BulletRigidbody.velocity = transform.forward * _moveSpeed - transform.up;
    }
}
