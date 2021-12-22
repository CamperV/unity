using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Perk : MonoBehaviour, IPerk
{
    public Unit boundUnit { get; set; }

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }
}