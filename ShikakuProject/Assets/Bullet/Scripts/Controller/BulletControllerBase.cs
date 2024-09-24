using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class BulletControllerBase : MonoBehaviour, IChaceable, IDamage, IStoppable
{
    protected Transform EnemyTransform { get; private set; }

    protected Rigidbody BulletRigidbody { get; private set; }

    protected bool IsStop { get; private set; } = false;


    public void Start()
    {
        BulletRigidbody = GetComponent<Rigidbody>();
    }


    //----------------------------------------------------------------------------------
    // EnemyTransform
    public virtual void SetEnemyTransform(Transform enemyTransform)
    {
        if (enemyTransform != null)
            EnemyTransform = enemyTransform;
        else
        {
            // null
            EnemyTransform = transform;
            EnemyTransform.position = transform.forward;
        }
    }

    //----------------------------------------------------------------------------------
    // 
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
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamage>().Damage(transform.position);
        }
    }
}