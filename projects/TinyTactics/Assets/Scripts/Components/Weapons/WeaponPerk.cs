using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class WeaponPerk : MonoBehaviour, IMutatorComponent
{
    public string displayName { get; set; }

    public Weapon boundWeapon { get; set; }

    void Awake() {
        boundWeapon = GetComponent<Weapon>();
    }

    public abstract void OnEquip();
    public abstract void OnUnequip();

    void OnDisable() => OnUnequip();
}