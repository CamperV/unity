using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Attack
{
    public readonly float criticalMultiplier = 3.0f;

    public int damage;
    public int hitRate;
    public int critRate;

    private Dictionary<string, string> advantages = new Dictionary<string, string>{
        ["WeaponSlash"]  = "WeaponBlunt",
        ["WeaponBlunt"]  = "WeaponPierce",
        ["WeaponPierce"] = "WeaponSlash"
    };

    public Attack(int dmg, int hit, int crit) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;

        Debug.Log($"Attack - DMG {damage}, HIT {hitRate}%, CRIT {critRate}%");
    }

    public override string ToString() { return $"Attack(dmg {damage}, hit {hitRate}%, crit {critRate}%)"; }

    public void Modify(Unit aggressor, Unit defender) {
        Debug.Assert(advantages.ContainsKey(aggressor.equippedWeapon.tag) && advantages.ContainsKey(defender.equippedWeapon.tag));

        if (defender.equippedWeapon.tag == advantages[aggressor.equippedWeapon.tag]) {
            damage   += 1;
            hitRate  += 15;
            critRate += 5;
        } else if (aggressor.equippedWeapon.tag == advantages[defender.equippedWeapon.tag]) {
            damage   -= 1;
            hitRate  -= 15;
            critRate -= 5;
        }

        Debug.Log($"Attack modified to {this}");
    }
}