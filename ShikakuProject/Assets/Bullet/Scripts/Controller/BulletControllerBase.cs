using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class BulletControllerBase : MonoBehaviour, IChaceable, IDamage, IStoppable, IDestroy, IBulletManaged
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
    private GameObject _effectExplosionPrefab;

    [SerializeField]
    private Collider[] _colliders;


    public void Start()
    {
        BulletRigidbody = GetComponent<Rigidbody>();
    }

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
    // 時間が経ったら消えるようにする関数
    [SerializeField]
    protected float _deliteTime = 1;

    float _deliteProgressTime = 0;

    private void DestroyAfter()
    {
        _deliteProgressTime += Time.deltaTime;

        if (_deliteProgressTime > _deliteTime)
        {
            _deliteProgressTime = 0;
            OnEnabled();
        }
    }


    //----------------------------------------------------------------------------------
    // 使用されるときに呼び出される関数
    public virtual void OnEnter(Transform enemyTransform)
    {
        MoveState = MoveStates.Nomal;

        SetEnemyTransform(enemyTransform);

        _soundManager.OnSpawn();

        foreach (Collider collider in _colliders)
            collider.enabled = true;
    }

    // EnemyTransformを設定する
    private void SetEnemyTransform(Transform enemyTransform)
    {
        if (enemyTransform != null)
            EnemyTransform = enemyTransform;
        else
        {
            // nullだった場合はエネミーのTransform情報もNullにする
            EnemyTransform = null;
        }
    }


    //----------------------------------------------------------------------------------
    // 敵の攻撃に当たって消えるときの関数
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
    // アクティブ状態を終了する関数
    protected virtual void OnEnabled()
    {
        MoveState = MoveStates.None;

        IsStop = false;

        if (_effectExplosionPrefab)
            Instantiate(_effectExplosionPrefab, transform.position, transform.rotation);

        OnDestroyHundle?.Invoke(this);
        OnBulletDestroy?.Invoke(gameObject);
    }


    //----------------------------------------------------------------------------------
    // 継承先に持ってほしい関数
    protected abstract void OnMove();


    //----------------------------------------------------------------------------------
    // IChaceable
    public Transform chacebleTransform { get { return transform; } }

    //----------------------------------------------------------------------------------
    // IDamage
    public void Damage()
    {
        if (MoveState != MoveStates.Nomal) return;

        MoveState = MoveStates.Death;

        _soundManager.OnHit();

        foreach (Collider collider in _colliders)
            collider.enabled = false;
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