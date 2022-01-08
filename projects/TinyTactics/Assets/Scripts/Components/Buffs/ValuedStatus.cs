using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class ValuedStatus : Status
{
    [SerializeField] public int modifierValue = 1;
    [SerializeField] public int expireTimer = 1;

    public abstract void OnAcquire();
    public abstract void OnExpire();

    void OnEnable() => OnAcquire();
    void OnDisable() => OnExpire();

    public void AddValue(int val) {
        modifierValue += val;
    }
    
    public void TakeBestTimer(int timer) {
        expireTimer = Mathf.Max(expireTimer, timer);
    }

    public void AddValuesAndReapply(int val, int timer) {
        OnExpire();
        //
        AddValue(val);
        TakeBestTimer(timer);
        //
        OnAcquire();
    }

    public void SetValuesAndReapply(int val, int timer) {
        OnExpire();
        //
        modifierValue = val;
        expireTimer = timer;
        //
        OnAcquire();
    }

    protected void TickExpire(Unit target) {
        expireTimer--;

        if (expireTimer <= 0) {
            UIManager.inst.combatLog.AddEntry($"{target.logTag}@[{target.displayName}]'s BLUE@[{displayName}] expired.");
            Destroy(this);
        }
    }
}