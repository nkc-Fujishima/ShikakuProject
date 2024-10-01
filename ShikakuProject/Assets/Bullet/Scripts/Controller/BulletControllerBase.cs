using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class BulletControllerBase : MonoBehaviour, IChaceable, IDamage, IStoppable, IDestroy, IBulletManaged
{
    protected Transform EnemyTransform { get; private set; }

    protected Rigidbody BulletRigidbody { get; private set; }

    protected bool IsStop { get; private set; } = false;

    [HideInInspector]
    public event Action<IChaceable> OnDestroyHundle;


    public void Start()
    {
        BulletRigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        DestroyAfter();
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
            Damage();
    }

    //----------------------------------------------------------------------------------
    // 使用されるときに呼び出される関数
    public virtual void OnEnter(Transform enemyTransform)
    {
        SetEnemyTransform(enemyTransform);
    }

    //----------------------------------------------------------------------------------
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
    // 継承先に持ってほしい関数
    protected abstract void OnMove();


    //----------------------------------------------------------------------------------
    // IChaceable
    public Transform chacebleTransform { get { return transform; } }

    //----------------------------------------------------------------------------------
    // IDamage
    public void Damage()
    {
        _deliteProgressTime = 0;

        OnDestroyHundle?.Invoke(this);
        OnBulletDestroy?.Invoke(gameObject);
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
            if (other.TryGetComponent<IDamage>(out IDamage sr))
                sr.Damage(transform.position);
        }
    }
}