using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Perk : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }
    public string displayName { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public abstract void OnAcquire();
    public abstract void OnRemoval();

    void OnDisable() => OnRemoval();
}