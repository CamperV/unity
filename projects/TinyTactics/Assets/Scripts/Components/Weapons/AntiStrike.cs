using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AntiStrike : WeaponPerk, IToolTip
{
    public string tooltipName { get; set; } = "Weapon Advantage (Strike)";
    public string tooltip { get; set; } = "+15 HIT, +15 AVOID, +15 CRITAVOID against Strike weapons.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnAttack += OffensiveAdv;
        boundWeapon.boundUnit.OnDefend += DefensiveAdv;

        displayName = "Weapon Advantage";
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnAttack -= OffensiveAdv;
        boundWeapon.boundUnit.OnDefend -= DefensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutAtt.hitRate += 15;
            //
            mutAtt.AddMutator(this);
        }
    }

    private void DefensiveAdv(ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.avoidRate       += 15;
            mutDef.critAvoidRate   += 15;
            //
            mutDef.AddMutator(this);
        }
    }
}