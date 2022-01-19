using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AntiStrike : WeaponPerk, IToolTip
{
    public override string displayName { get; set; } = "Weapon Advantage";

    public int dmgModRate = 1;
    public int hitModRate = 15;
    public int critModRate = 5;
    
    // IToolTip
    public string tooltipName { get; set; } = "Weapon Advantage (Strike)";
    public string tooltip => $"+{dmgModRate} ATK/DEF, +{hitModRate} HIT/AVO, +{critModRate} CRIT/CRITAVO against Strike weapons.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnAttack += OffensiveAdv;
        boundWeapon.boundUnit.OnDefend += DefensiveAdv;
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnAttack -= OffensiveAdv;
        boundWeapon.boundUnit.OnDefend -= DefensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        if (mutAtt.inMeleeRange && target.equippedWeapon.HasTagMatch("Strike")) {
            mutAtt.damage += dmgModRate;
            mutAtt.hitRate += hitModRate;
            mutAtt.critRate += critModRate;
            //
            mutAtt.AddMutator(this);
        }
    }

    private void DefensiveAdv(ref MutableDefense mutDef, Unit target) {
        if (mutDef.inMeleeRange && target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.damageReduction += dmgModRate;
            mutDef.avoidRate += hitModRate;
            mutDef.critAvoidRate += critModRate;
            //
            mutDef.AddMutator(this);
        }
    }
}