using System;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour, IStateChangeable, IDamage, IFallable
{
    [Header("オブジェクト設定"), Tooltip("エネミーのパラメータスクリプタブルオブジェクト"), SerializeField] protected EnemyParameterDataBase parameter;
    [Tooltip("エネミーで使用するエフェクト群スクリプタブルオブジェクト"), SerializeField] protected EnemyEffectDataBase effect;
    [Tooltip("エネミー用カーソル"), SerializeField] protected CursorController cursor;

    protected IState iState = null;

    // 全エネミー共通のやられた処理のステート
    private DieState dieState = null;

    protected Rigidbody rigidBody = null;

    protected Animator animator = null;

    protected AudioSource audioSource = null;

    private CapsuleCollider[] colliders = null;

    public event Action<EnemyControllerBase> OnDestroyHundle = null;
    protected void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        colliders = GetComponents<CapsuleCollider>();

        dieState = new DieState(this.gameObject, animator, rigidBody, colliders, audioSource, parameter, effect);

        // 敵の頭上に出すカーソル生成
        CursorController cursorController = Instantiate(cursor);
        cursorController?.Construct(transform);
    }

    public virtual void OnStart() { }

    public void ChangeState(IState nextState)
    {
        if (iState != null) iState.OnExit();

        iState = nextState;

        if (iState != null) iState.OnEnter();
    }

    // ダメージを食らった場合の処理
    public void Damage(Vector3 position)
    {
        // ダメージを与えてくる対象へのベクトルと自身の前方ベクトルで内積
        Vector2 forwardVector2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 targetVector2D = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;

        float dotAngle = MathF.Acos((forwardVector2D.x * targetVector2D.x) + (forwardVector2D.y * targetVector2D.y) / (MathF.Sqrt(MathF.Pow(forwardVector2D.x, 2) + MathF.Pow(forwardVector2D.y, 2)) * MathF.Sqrt(MathF.Pow(targetVector2D.x, 2) + MathF.Pow(targetVector2D.y, 2))));
        float angle = dotAngle * Mathf.Rad2Deg;

        // パラメータ内のダメージを受けない角度以上のみダメージを受けてイベントを発火
        if (angle < parameter.InvincibleAngle) return;

        Death();
    }

    // ステージから落ちた場合の処理
    public void FallRiver()
    {
        Death();
    }

    // 死ぬときの処理
    private void Death()
    {
        ChangeState(dieState);

        //this.gameObject.SetActive(false);

        OnDestroyHundle?.Invoke(this);
    }

    // 全エネミー共通やられた場合の処理
    protected class DieState : IState
    {
        GameObject gameObject = null;
        Animator animator = null;
        Rigidbody rigidbody = null;
        CapsuleCollider[] colliders = null;
        AudioSource audioSource = null;

        EnemyParameterDataBase parameter = null;
        EnemyEffectDataBase effect = null;

        const float effectPosY = 1.5f;

        float countTime = 0;
        public DieState(GameObject gameObject, Animator animator, Rigidbody rigidbody, CapsuleCollider[] colliders, AudioSource audioSource, EnemyParameterDataBase parameter, EnemyEffectDataBase effect)
        {
            this.gameObject = gameObject;
            this.animator = animator;
            this.rigidbody = rigidbody;
            this.colliders = colliders;
            this.audioSource = audioSource;
            this.parameter = parameter;
            this.effect = effect;
        }

        public void OnEnter()
        {
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            rigidbody.isKinematic = true;
            animator?.Play("Die", 0, 0.0f);

            rigidbody?.AddForce(gameObject.transform.forward * parameter.DownForcePower, ForceMode.Impulse);
            audioSource.clip = effect.DownSE;
            audioSource?.Play();

            Instantiate(effect.DownEffect, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + effectPosY, gameObject.transform.position.z), Quaternion.identity);
        }

        public void OnExit()
        {
        }

        public void OnUpdate()
        {
            countTime += Time.deltaTime;

            if (countTime > parameter.ToDestroyTime)
            {
                Instantiate(effect.DestroyEffect, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + effectPosY, gameObject.transform.position.z), Quaternion.Euler(90, 0, 0));
                DestroyAudioPlay.PlayClipAtPoint(effect.DestroySE, gameObject.transform.position, 1f);

                gameObject.SetActive(false);
            }
        }
    }
}
