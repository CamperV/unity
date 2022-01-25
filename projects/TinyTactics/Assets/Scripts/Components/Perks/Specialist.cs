using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Specialist : Perk, IToolTip
{
    public override string displayName { get; set; } = "Specialist";

    // IToolTip
    public string tooltipName { get; set; } = "Specialist";
    public string tooltip => "Gain Weapon Advantage against enemies using the same Weapon Type.";

    public override void OnAcquire() {
        boundUnit.OnAttack += OffensiveAdv;
        boundUnit.OnDefend += DefensiveAdv;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= OffensiveAdv;
        boundUnit.OnDefend -= DefensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        foreach (string tag in boundUnit.equippedWeapon.tags) {
            if (mutAtt.inMeleeRange && target.equippedWeapon.HasTagMatch(tag)) {
                mutAtt.damage += 1;
                mutAtt.hitRate += 15;
                mutAtt.critRate += 15;
                //
                mutAtt.AddMutator(this);
            }
        }
    }

    private void DefensiveAdv(ref MutableDefense mutDef, Unit target) {
        foreach (string tag in boundUnit.equippedWeapon.tags) {
            if (mutDef.inMeleeRange && target.equippedWeapon.HasTagMatch(tag)) {
                mutDef.damageReduction += 1;
                mutDef.avoidRate += 15;
                mutDef.critAvoidRate += 15;
                //
                mutDef.AddMutator(this);
            }
        }
    }
}