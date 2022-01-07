using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BonyResistance : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Oops, All Bones";
    public string tooltip { get; set; } = "Add 3 DEF against Slashing and Piercing weapons, but -3 DEF against Striking weapons.";

    public override void OnAcquire() {
        boundUnit.OnDefend += ConditionalDefense;
        //
        displayName = "Oops, All Bones";
    }

    public override void OnRemoval() {
        boundUnit.OnDefend -= ConditionalDefense;
    }

    private void ConditionalDefense(ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Slash", "Pierce")) {
            mutDef.damageReduction += 3;
            //
            mutDef.AddMutator(this);
        }
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.damageReduction -= 3;
            //
            mutDef.AddMutator(this);
        }
    }
}