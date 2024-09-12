using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour,IStateChangeable
{
    protected IState iState = null;

    protected Animator animator = null;

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
}
