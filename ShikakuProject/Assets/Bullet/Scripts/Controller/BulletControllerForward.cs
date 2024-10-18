using UnityEngine;

// 真っ直ぐ進む弾
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
    // 移動関数
    protected override void OnMove()
    {
        if (MoveState != MoveStates.Nomal) return;

        if (IsStop) return;

        Vector3 movePower = transform.forward * _moveSpeed - transform.up;

        BulletRigidbody.velocity = new Vector3(movePower.x, BulletRigidbody.velocity.y, movePower.z);
    }


    //----------------------------------------------------------------------------------
    // 使用されるときに呼び出される関数
    public override void OnEnter(Transform enemyTransform)
    {
        _animator.SetInteger("Walk", 1);

        base.OnEnter(enemyTransform);
    }

    //----------------------------------------------------------------------------------
    // アクティブ状態を終了する関数
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
