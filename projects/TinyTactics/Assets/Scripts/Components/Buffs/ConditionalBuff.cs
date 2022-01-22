using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class ConditionalBuff : ValuedStatus
{
    public Func<bool> ConditionValid;

    protected void CheckCondition() {
        if (!ConditionValid()) ExpireImmediately(boundUnit);
    }

    public void ApplyValueAndCondition(int val, Func<bool> Condition) {
        OnExpire();
        //
        modifierValue = val;
        ConditionValid = Condition;
        //
        OnAcquire();
    }
}