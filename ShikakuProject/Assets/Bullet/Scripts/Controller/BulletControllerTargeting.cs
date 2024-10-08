using UnityEngine;

// 敵の方向に真っ直ぐ進む弾
public class BulletControllerTargeting : BulletControllerBase
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField]
    private Animator _animator;

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
    // EnemyTransformを設定する関数
    public override void OnEnter(Transform enemyTransform)
    {
        base.OnEnter(enemyTransform);

        OnFaceTarget();
    }

    //----------------------------------------------------------------------------------
    // 移動関数
    protected override void OnMove()
    {
        if (IsStop) return;

        Vector3 movePower = transform.forward * _moveSpeed - transform.up;

        BulletRigidbody.velocity = new Vector3(movePower.x, BulletRigidbody.velocity.y, movePower.z);
    }

    //----------------------------------------------------------------------------------
    // 敵の方向に向く
    private void OnFaceTarget()
    {
        if (!EnemyTransform) return;

        Vector3 vectolEnemyToBullet = EnemyTransform.position - transform.position;

        float moveRotationY = Mathf.Atan2(-vectolEnemyToBullet.z, vectolEnemyToBullet.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, moveRotationY, 0);
    }
}