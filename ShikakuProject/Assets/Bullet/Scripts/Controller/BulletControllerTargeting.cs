using UnityEngine;

// �G�̕����ɐ^�������i�ޒe
public class BulletControllerTargeting : BulletControllerBase, IDamage, IStoppable
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField]
    private Animator _animator;


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
    // �G�̕����Ɍ���
    private void OnFaceTarget()
    {
        if (!EnemyTransform) return;

        Vector3 vectolEnemyToBullet = EnemyTransform.position - transform.position;

        float moveRotationY = Mathf.Atan2(-vectolEnemyToBullet.z, vectolEnemyToBullet.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, moveRotationY, 0);
    }


    //----------------------------------------------------------------------------------
    // �g�p�����Ƃ��ɌĂяo�����֐�
    public override void OnEnter(Transform enemyTransform)
    {
        _animator.SetInteger("Walk", 1);

        base.OnEnter(enemyTransform);

        OnFaceTarget();
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