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
}