using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour, IStateChangeable, IDamage
{
    [Header("オブジェクト設定"), SerializeField] protected EnemyParameterBase parameter;

    protected IState iState = null;

    protected Animator animator = null;

    public event Action OnDestroyHundle = null;
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeState(IState nextState)
    {
        if (iState != null) iState.OnExit();

        iState = nextState;

        if (iState != null) iState.OnEnter();
    }

    public void Damage(Vector3 position)
    {
        // ダメージを与えてくる対象へのベクトルと自身の前方ベクトルで内積
        Vector2 forwardVector2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 targetVector2D = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;

        float dotAngle = MathF.Acos((forwardVector2D.x * targetVector2D.x) + (forwardVector2D.y * targetVector2D.y) / (MathF.Sqrt(MathF.Pow(forwardVector2D.x, 2) + MathF.Pow(forwardVector2D.y, 2)) * MathF.Sqrt(MathF.Pow(targetVector2D.x, 2) + MathF.Pow(targetVector2D.y, 2))));
        float angle = dotAngle * Mathf.Rad2Deg;

        // パラメータ内のダメージを受けない角度以上のみダメージを受けてイベントを発火
        if (angle < parameter.InvincibleAngle) return;

        Debug.Log("エネミーがダメージを受けたよ");

        this.gameObject.SetActive(false);

        OnDestroyHundle?.Invoke();
    }
}
