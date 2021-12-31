using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BonyResistance : Perk
{
    void OnEnable() {
        boundUnit.OnDefend += ConditionalDefense;
    }

    private void ConditionalDefense(ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Slash", "Pierce")) {
            mutDef.damageReduction += 3;
            //
            mutDef.mutators.Add(this.GetType().Name);
        }
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.damageReduction -= 3;
            //
            mutDef.mutators.Add(this.GetType().Name);
        }
    }
}