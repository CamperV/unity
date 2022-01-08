using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Status : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }
    public virtual string displayName { get; set; }

    public string provider = "Emaculate Provision a.k.a. ERROR";

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public Status WithProvider(string _provider) {
        provider = _provider;
        return this;
    }
}