using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class ValuedStatus : Status
{
    [SerializeField] public int modifierValue = 1;

    public abstract void OnAcquire();
    public abstract void OnExpire();

    void OnEnable() => OnAcquire();
    void OnDisable() => OnExpire();

    public virtual string affectedStat => "";

    public void AddValue(int val) {
        modifierValue += val;
    }

    public void AddValuesAndReapply(int val) {
        OnExpire();
        //
        AddValue(val);
        //
        OnAcquire();
    }

    public void SetValuesAndReapply(int val) {
        OnExpire();
        //
        modifierValue = val;
        //
        OnAcquire();
    }

    protected void TickExpire(Unit target) {
        int nextIncr = (int)Mathf.MoveTowards(modifierValue, 0f, 1f);
        SetValuesAndReapply(nextIncr);

        if (modifierValue == 0) {
            Destroy(this);
        }
    }

    protected void ExpireImmediately(Unit target) {
        Destroy(this);
    }
}