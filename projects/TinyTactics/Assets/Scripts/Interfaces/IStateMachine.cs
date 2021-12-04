using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public interface IStateMachine<E>
{
    void ChangeState(E e);
    void EnterState(E e);
    void ExitState(E e);
}