using UnityEngine;

// ���̏�ɗ��܂�e
public class BulletControllerIdle : BulletControllerBase, IDamage, IStoppable
{
    [SerializeField]
    private Animator _animator;


    //----------------------------------------------------------------------------------
    // �ړ��֐�
    protected override void OnMove()
    {

    }

    //----------------------------------------------------------------------------------
    // �g�p�����Ƃ��ɌĂяo�����֐�
    public override void OnEnter(Transform enemyTransform)
    {
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
