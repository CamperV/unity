using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class ConditionalBuff : Buff
{
    public Func<bool> ConditionValid;

    protected void CheckCondition() {
        if (!ConditionValid()) TickExpire(boundUnit);
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