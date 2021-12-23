using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Perk : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }
}