using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class CoupledBuff : ValuedStatus
{
    public Unit coupledTarget;

    public void ApplyValueAndCoupling(int val, Unit _coupledTarget) {
        OnExpire();
        //
        modifierValue = val;
        coupledTarget = _coupledTarget;
        //
        OnAcquire();
    }
}