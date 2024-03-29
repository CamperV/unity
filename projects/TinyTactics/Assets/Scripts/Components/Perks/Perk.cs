using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Perk : MonoBehaviour
{
    public Unit boundUnit { get; set; }

    // set in inspector
    // OR
    // set by Instantiator
    [field: SerializeField] public PerkData PerkData { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public abstract void OnAcquire();
    public abstract void OnRemoval();

    void OnDisable() => OnRemoval();
}