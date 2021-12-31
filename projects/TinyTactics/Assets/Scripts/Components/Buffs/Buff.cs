using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Buff : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }
    public virtual string displayName { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public abstract void OnAcquire();
    public abstract void OnExpire();
    public abstract void Increment();

    void OnEnable() => OnAcquire();
    // void OnDisable() => OnExpire();
}