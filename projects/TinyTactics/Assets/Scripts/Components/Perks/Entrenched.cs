using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Entrenched : Perk, IToolTip
{
    public override string displayName { get; set; } = "Entrenched";

    // IToolTip
    public string tooltipName { get; set; } = "Entrenched";
    public string tooltip { get; set; } = "If this unit has not moved since last turn, increase DMG, HIT, and CRIT.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        if (boundUnit.moveAvailable) {
            mutAtt.damage += 3;
            mutAtt.hitRate += 25;
            mutAtt.critRate += 15;
            //
            mutAtt.AddMutator(this);
        }
    }
}