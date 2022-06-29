using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(WeaponStats))]
public class MartialWeapon : _Weapon
{
    public override int CalculateDamage() {
        // first pass
        return weaponStats.MIGHT;
    }
}