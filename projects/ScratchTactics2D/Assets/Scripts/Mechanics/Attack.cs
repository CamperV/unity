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

    private static Dictionary<string, string> advantages = new Dictionary<string, string>{
        ["slash"]   = "strike",
        ["strike"]  = "pierce",
        ["pierce"]  = "slash",
        //
        ["missile"] = "n/a"
    };

    public Attack(int dmg, int hit, int crit) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;

        Debug.Log($"Attack - DMG {damage}, HIT {hitRate}%, CRIT {critRate}%");
    }
    public Attack(Attack toClone) {
        damage = toClone.damage;
        hitRate = toClone.hitRate;
        critRate = toClone.critRate;
    }

    public override string ToString() { return $"Attack(dmg {damage}, hit {hitRate}%, crit {critRate}%)"; }

    public static Attack Modify(Attack attack, Unit aggressor, Unit defender) {
        Attack retVal = new Attack(attack);

        // "Weapon Triangle" modifications
        if (HasAdvantage(aggressor.equippedWeapon, defender.equippedWeapon)) {
            retVal.damage   += 1;
            retVal.hitRate  += 15;
            retVal.critRate += 5;
        } else if (HasAdvantage(defender.equippedWeapon, aggressor.equippedWeapon)) {
            retVal.damage   -= 1;
            retVal.hitRate  -= 15;
            retVal.critRate -= 5;
        }

        // stat modifications
        retVal.hitRate  -= defender.REFLEX*2;
		retVal.critRate -= defender.REFLEX;

        return retVal;
    }

    private static bool HasAdvantage(Weapon A, Weapon B) {
        foreach (string tag in A.tags) {
            if (advantages.ContainsKey(tag)) {
                if (B.tags.Contains(advantages[tag]))
                    return true;
            }
        }
        return false;
    }
}