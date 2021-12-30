using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AntiStrike : WeaponPerk
{
    public override void OnEquip() {
        boundWeapon.boundUnit.OnAttack += OffensiveAdv;
        boundWeapon.boundUnit.OnDefend += DefensiveAdv;
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnAttack -= OffensiveAdv;
        boundWeapon.boundUnit.OnDefend -= DefensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutAtt.hitRate += 15;
        }
    }

    private void DefensiveAdv(ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.avoidRate       += 15;
            mutDef.critAvoidRate   += 15;
        }
    }
}