using UnityEngine;

// �^�������i�ޒe
public class BulletControllerForward : BulletControllerBase, IDamage, IStoppable
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField]
    Animator _animator;


    private new void Update()
    {
        base.Update();
        OnMove();
    }

    //----------------------------------------------------------------------------------
    // �ړ��֐�
    protected override void OnMove()
    {
        if (MoveState != MoveStates.Nomal) return;

        if (IsStop) return;

        Vector3 movePower = transform.forward * _moveSpeed - transform.up;

        BulletRigidbody.velocity = new Vector3(movePower.x, BulletRigidbody.velocity.y, movePower.z);
    }


    //----------------------------------------------------------------------------------
    // �g�p�����Ƃ��ɌĂяo�����֐�
    public override void OnEnter(Transform enemyTransform)
    {
        _animator.SetInteger("Walk", 1);

        base.OnEnter(enemyTransform);
    }

    //----------------------------------------------------------------------------------
    // �A�N�e�B�u��Ԃ��I������֐�
    protected override void OnEnabled()
    {
        _animator.SetBool("Death", false);

        base.OnEnabled();
    }


    //----------------------------------------------------------------------------------
    // IDamage
    public new void Damage()
    {
        if (MoveState != MoveStates.Nomal) return;

        _animator.SetBool("Death", true);

        base.Damage();
    }

    //----------------------------------------------------------------------------------
    // IStoppable
    public new void OnStop()
    {
        _animator.SetInteger("Walk", 0);
        base.OnStop();
    }
}
