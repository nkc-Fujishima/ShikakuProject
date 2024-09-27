using UnityEngine;

// �G�̕����ɐ^�������i�ޒe
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
    // EnemyTransform��ݒ肷��֐�
    public override void OnEnter(Transform enemyTransform)
    {
        base.OnEnter(enemyTransform);

        OnFaceTarget();
    }

    //----------------------------------------------------------------------------------
    // �ړ��֐�
    protected override void OnMove()
    {
        if (IsStop) return;
        if (!EnemyTransform) return;

        BulletRigidbody.velocity = transform.forward * _moveSpeed - transform.up;
    }

    //----------------------------------------------------------------------------------
    // �G�̕����Ɍ���
    private void OnFaceTarget()
    {
        Vector3 vectolEnemyToBullet = EnemyTransform.position - transform.position;

        float moveRotationY = Mathf.Atan2(-vectolEnemyToBullet.z, vectolEnemyToBullet.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, moveRotationY, 0);
    }
}