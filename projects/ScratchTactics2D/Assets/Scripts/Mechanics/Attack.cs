using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Attack
{
    public readonly float criticalMultiplier = 3.0f;

    private int _damage;
    public int damage {
        get => _damage;
        set { _damage = (value < 0) ? 0 : value; }
    }
    private int _hitRate;
    public int hitRate {
        get => _hitRate;
        set { _hitRate = (value < 0) ? 0 : value; }
    }
    private int _critRate;
    public int critRate {
        get => _critRate;
        set { _critRate = (value < 0) ? 0 : value; }
    }

    private Dictionary<string, string> advantages = new Dictionary<string, string>{
        ["SlashWeapon"]   = "BluntWeapon",
        ["BluntWeapon"]   = "PierceWeapon",
        ["PierceWeapon"]  = "SlashWeapon",
        //
        ["MissileWeapon"] = "n/a"
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

        // "Weapon Triangle" modifications
        if (defender.equippedWeapon.tag == advantages[aggressor.equippedWeapon.tag]) {
            damage   += 1;
            hitRate  += 15;
            critRate += 5;
        } else if (aggressor.equippedWeapon.tag == advantages[defender.equippedWeapon.tag]) {
            damage   -= 1;
            hitRate  -= 15;
            critRate -= 5;
        }

        // stat modifications
        hitRate  -= defender.REFLEX*2;
		critRate -= defender.REFLEX;

        Debug.Log($"Attack modified to {this}");
    }
}