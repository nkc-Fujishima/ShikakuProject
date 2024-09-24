using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract  class BulletControllerBase : MonoBehaviour, IChaceable, IDamage, IStoppable
{
    protected Transform EnemyTransform { get; private set; }

    protected Rigidbody BulletRigidbody { get; private set; }

    protected bool IsStop { get; private set; } = false;


    public void Start()
    {
        BulletRigidbody = GetComponent<Rigidbody>();
    }


    //----------------------------------------------------------------------------------
    // EnemyTransformを設定する関数
    public virtual void SetEnemyTransform(Transform enemyTransform)
    {
        if (enemyTransform != null)
            EnemyTransform = enemyTransform;
        else
        {
            // nullだった場合はそのオブジェクトの眼の前に設定する
            EnemyTransform = transform;
            EnemyTransform.position = transform.forward;
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
        Destroy(gameObject);
    }

    //----------------------------------------------------------------------------------
    // IStoppable
    public void OnStop()
    {
        IsStop = true;
    }


    //----------------------------------------------------------------------------------
    // Trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamage>().Damage(transform.position);
        }
    }
}
