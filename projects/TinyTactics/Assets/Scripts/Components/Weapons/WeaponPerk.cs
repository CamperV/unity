using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class WeaponPerk : MonoBehaviour, IMutatorComponent
{
    public abstract string displayName { get; set; }

    public _Weapon boundWeapon { get; set; }

    void Awake() {
        boundWeapon = GetComponent<_Weapon>();
    }

    public abstract void OnEquip();
    public abstract void OnUnequip();

    void OnDisable() => OnUnequip();
}