using UnityEngine;

// その場に留まる弾
public class BulletControllerIdle : BulletControllerBase, IDamage, IStoppable
{
    [SerializeField]
    private Animator _animator;


    //----------------------------------------------------------------------------------
    // 移動関数
    protected override void OnMove()
    {

    }

    //----------------------------------------------------------------------------------
    // 使用されるときに呼び出される関数
    public override void OnEnter(Transform enemyTransform)
    {
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
