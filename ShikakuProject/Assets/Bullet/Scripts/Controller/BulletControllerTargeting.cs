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

    private void Update()
    {
        OnMove();
    }


    //----------------------------------------------------------------------------------
    // EnemyTransformを設定する関数
    public override void SetEnemyTransform(Transform enemyTransform)
    {
        base.SetEnemyTransform(enemyTransform);

        OnFaceTarget();
    }

    //----------------------------------------------------------------------------------
    // 移動関数
    protected override void OnMove()
    {
        if (IsStop) return;
        if (!EnemyTransform) return;

        BulletRigidbody.velocity = transform.forward * _moveSpeed;
    }

    //----------------------------------------------------------------------------------
    // 敵の方向に向く
    private void OnFaceTarget()
    {
        Vector3 vectolEnemyToBullet = EnemyTransform.position - transform.position;

        float moveRotationY = Mathf.Atan2(-vectolEnemyToBullet.z, vectolEnemyToBullet.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, moveRotationY, 0);
    }
}