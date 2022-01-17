using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Perk : MonoBehaviour, IMutatorComponent
{
    public static readonly List<string> ValidArchetypes = new List<string>{
        "Assault",
        "Defender",
        "Support",
        "Cunning",
        "Quick"
    };

    public Unit boundUnit { get; set; }
    public abstract string displayName { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    public abstract void OnAcquire();
    public abstract void OnRemoval();

    void OnDisable() => OnRemoval();
}