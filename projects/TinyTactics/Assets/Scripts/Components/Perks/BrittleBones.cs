using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BrittleBones : Perk, IToolTip
{
    public override string displayName { get; set; } = "Brittle Bones";

    // IToolTip
    public string tooltipName { get; set; } = "Brittle Bones";
    public string tooltip { get; set; } = "+4 DEFENSE. On hurt, reduce DEFENSE by 1. Recieves +4 DMG from Striking weapons.";

    public override void OnAcquire() {
        boundUnit.statusManager.AddValuedStatus<ReducibleDefenseBuff>("Brittle Bones", 4);
        boundUnit.OnDefend += ConditionalDefense;
    }

    public override void OnRemoval() {
        boundUnit.statusManager.RemoveAllStatusFromProvider("Brittle Bones");
        boundUnit.OnDefend -= ConditionalDefense;
    }

    // receive more dmg from striking weapons
    private void ConditionalDefense(ref MutableDefense mutDef, Unit target) {
        if (target.equippedWeapon.HasTagMatch("Strike")) {
            mutDef.damageReduction -= 4;
            //
        }
        mutDef.AddMutator(this);
    }
}