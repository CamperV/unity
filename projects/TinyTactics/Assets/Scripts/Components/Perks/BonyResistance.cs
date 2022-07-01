using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BonyResistance : Perk, IToolTip
{
    public override string displayName { get; set; } = "Oops, All Bones";

    // IToolTip
    public string tooltipName { get; set; } = "Oops, All Bones";
    public string tooltip { get; set; } = "Add 2 DEF against Slashing and Piercing weapons, but -4 DEF against Striking weapons.";

    public override void OnAcquire() {
        boundUnit.OnDefend += ConditionalDefense;
    }

    public override void OnRemoval() {
        boundUnit.OnDefend -= ConditionalDefense;
    }

    private void ConditionalDefense(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Slash", "Pierce")) {
            mutDef.damageReduction += 2;
            //
            mutDef.AddMutator(this);
        }
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.damageReduction -= 4;
            //
            mutDef.AddMutator(this);
        }
    }
}