using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateChangeable
{
    public void ChangeState(IState iState);
}
