using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class WeaponPerk : MonoBehaviour
{
    public Weapon boundWeapon { get; set; }

    void Awake() {
        boundWeapon = GetComponent<Weapon>();
    }

    public abstract void OnEquip();
    public abstract void OnUnequip();
}