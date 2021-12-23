using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(WeaponStats))]
public class Weapon : MonoBehaviour, IMutatorComponent
{
    public Unit boundUnit { get; set; }

    // assigned in inspector or otherwise
    public Sprite sprite;
    public Color color;
    
    [HideInInspector] public WeaponStats weaponStats;

    void Awake() {
        boundUnit = GetComponent<Unit>();
        weaponStats = GetComponent<WeaponStats>();
    }
}