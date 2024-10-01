using UnityEngine;

// “G‚Ì•ûŒü‚É^‚Á’¼‚®i‚Ş’e
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
    // EnemyTransform‚ğİ’è‚·‚éŠÖ”
    public override void OnEnter(Transform enemyTransform)
    {
        base.OnEnter(enemyTransform);

        OnFaceTarget();
    }

    //----------------------------------------------------------------------------------
    // ˆÚ“®ŠÖ”
    protected override void OnMove()
    {
        if (IsStop) return;

        Vector3 movePower = transform.forward * _moveSpeed - transform.up;

        BulletRigidbody.velocity = new Vector3(movePower.x, BulletRigidbody.velocity.y, movePower.z);
    }

    //----------------------------------------------------------------------------------
    // “G‚Ì•ûŒü‚ÉŒü‚­
    private void OnFaceTarget()
    {
        if (!EnemyTransform) return;

        Vector3 vectolEnemyToBullet = EnemyTransform.position - transform.position;

        float moveRotationY = Mathf.Atan2(-vectolEnemyToBullet.z, vectolEnemyToBullet.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, moveRotationY, 0);
    }
}