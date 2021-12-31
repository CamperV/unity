using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BonyResistance : Perk
{
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