using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DuelistsBlow : Perk, IToolTip
{
    public override string displayName { get; set; } = "Duelist's Blow";

    // IToolTip
    public string tooltipName { get; set; } = "Duelist's Blow";
    public string tooltip { get; set; } = "When initiating combat, +15 AVOID. (Player Phase)";

    public override void OnAcquire() {
        boundUnit.OnDefend += ConditionalDefense;
    }

    public override void OnRemoval() {
        boundUnit.OnDefend -= ConditionalDefense;
    }

    private void ConditionalDefense(ref MutableDefense mutDef, Unit target) {
        if (boundUnit.turnActive) {
            mutDef.avoidRate += 15;
            //
            mutDef.AddMutator(this);
        }
    }
}