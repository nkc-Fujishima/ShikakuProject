using UnityEngine;

// ê^Ç¡íºÇÆêiÇﬁíe
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

    private void Update()
    {
        OnMove();
    }

    //----------------------------------------------------------------------------------
    // à⁄ìÆä÷êî
    protected override void OnMove()
    {
        if (IsStop) return;

        BulletRigidbody.velocity = transform.forward * _moveSpeed;
    }
}
