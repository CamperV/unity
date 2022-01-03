using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// What do buffs do?
// Generally, they are provided by other entities (sometimes Perks),
// but Destroy themselves after conditions are met
public abstract class Buff : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }
    public virtual string displayName { get; set; }

    public string provider = "Emaculate Provision a.k.a. ERROR";
    
    [SerializeField] public int buffValue = 1;
    [SerializeField] public int expireTimer = 1;

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public abstract void OnAcquire();
    public abstract void OnExpire();

    void OnEnable() => OnAcquire();
    void OnDisable() => OnExpire();

    public Buff WithProvider(string _provider) {
        provider = _provider;
        return this;
    }

    public void AddValue(int val) {
        buffValue += val;
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
        buffValue = val;
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