using UnityEngine;

// 真っ直ぐ進む弾
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
    // 移動関数
    protected override void OnMove()
    {
        if (IsStop) return;

        BulletRigidbody.velocity = transform.forward * _moveSpeed - transform.up;
    }
}
