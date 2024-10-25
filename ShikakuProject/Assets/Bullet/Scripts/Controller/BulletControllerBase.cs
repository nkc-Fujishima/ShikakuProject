using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class BulletControllerBase : MonoBehaviour, IChaceable, IDamage, IStoppable, IDestroy, IBulletManaged, IFallable
{
    protected enum MoveStates
    {
        Nomal,
        Death,
        None
    }

    protected MoveStates MoveState = MoveStates.None;

    protected Transform EnemyTransform { get; private set; }

    protected Rigidbody BulletRigidbody { get; private set; }

    protected bool IsStop { get; private set; } = false;

    [HideInInspector]
    public event Action<IChaceable> OnDestroyHundle;

    [SerializeField]
    internal BulletSoundManager _soundManager;

    [SerializeField]
    private ParticleSystem _particleExplosion;
    
    [SerializeField]
    private ParticleSystem _particleDamage;

    [SerializeField]
    private Collider[] _colliders;


    public void Update()
    {
        switch (MoveState)
        {
            case MoveStates.Nomal:
                DestroyAfter();
                break;
            case MoveStates.Death:
                OnDestroyMove();
                break;
        }
    }


    //----------------------------------------------------------------------------------
    // ���Ԃ��o�����������悤�ɂ���֐�
    [SerializeField]
    protected float _deliteTime = 1;

    float _deliteProgressTime = 0;

    private void DestroyAfter()
    {
        _deliteProgressTime += Time.deltaTime;

        if (_deliteProgressTime > _deliteTime)
        {
            OnEnabled();
        }
    }


    //----------------------------------------------------------------------------------
    // �g�p�����Ƃ��ɌĂяo�����֐�
    public virtual void OnEnter(Transform enemyTransform)
    {
        MoveState = MoveStates.Nomal;

        SetEnemyTransform(enemyTransform);

        _soundManager.OnSpawn();

        foreach (Collider collider in _colliders)
            collider.enabled = true;

        BulletRigidbody = GetComponent<Rigidbody>();
        BulletRigidbody.isKinematic = false;
    }

    // EnemyTransform��ݒ肷��
    private void SetEnemyTransform(Transform enemyTransform)
    {
        if (enemyTransform != null)
            EnemyTransform = enemyTransform;
        else
        {
            // null�������ꍇ�̓G�l�~�[��Transform����Null�ɂ���
            EnemyTransform = null;
        }
    }


    //----------------------------------------------------------------------------------
    // �G�̍U���ɓ������ď�����Ƃ��̊֐�
    private float _timeDestroyCount = 0;

    private const float TIME_DSTROY = 0.9f;

    protected virtual void OnDestroyMove()
    {
        _timeDestroyCount += Time.deltaTime;

        if (_timeDestroyCount > TIME_DSTROY)
        {
            _timeDestroyCount = 0;
            OnEnabled();
        }
    }


    //----------------------------------------------------------------------------------
    // �A�N�e�B�u��Ԃ��I������֐�
    protected virtual void OnEnabled()
    {
        MoveState = MoveStates.None;

        IsStop = false;

        _deliteProgressTime = 0;

        Instantiate(_particleExplosion, transform.position, _particleExplosion.transform.rotation);
        _soundManager.OnDeath(transform.position);

        OnDestroyHundle?.Invoke(this);
        OnBulletDestroy?.Invoke(gameObject);
    }


    //----------------------------------------------------------------------------------
    // ���ʂƂ��̏���
    private void Death()
    {
        if (MoveState != MoveStates.Nomal) return;

        MoveState = MoveStates.Death;

        _soundManager.OnHit();

        foreach (Collider collider in _colliders)
            collider.enabled = false;

        BulletRigidbody.isKinematic = true;

        if (_particleDamage)
            Instantiate(_particleDamage, transform.position + transform.up, Quaternion.identity);
    }


    //----------------------------------------------------------------------------------
    // �p����Ɏ����Ăق����֐�
    protected abstract void OnMove();


    //----------------------------------------------------------------------------------
    // IChaceable
    public Transform chacebleTransform { get { return transform; } }

    //----------------------------------------------------------------------------------
    // IDamage
    public void Damage()
    {
        Death();
    }

    //----------------------------------------------------------------------------------
    // IFallable
    public void FallRiver()
    {
        Death();
    }

    //----------------------------------------------------------------------------------
    // IStoppable
    public void OnStop()
    {
        IsStop = true;
    }

    //----------------------------------------------------------------------------------
    // IBulletManaged
    [SerializeField]
    private int typeCount = 0;

    [HideInInspector]
    public UnityEvent<GameObject> OnBulletDestroy;

    public void SetTypeCount(int typeCount)
    {
        this.typeCount = typeCount;
    }

    public int GetTypeCount()
    {
        return typeCount;
    }


    //----------------------------------------------------------------------------------
    // Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (MoveState != MoveStates.Nomal) return;

            if (other.TryGetComponent<IDamage>(out IDamage sr))
                sr.Damage(transform.position);
        }
    }
}