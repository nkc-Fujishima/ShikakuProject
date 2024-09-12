using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : IState
{
    IState iState = null;

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnUpdate();
}
