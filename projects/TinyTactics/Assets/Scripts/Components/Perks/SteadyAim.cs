using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SteadyAim : Perk, IToolTip
{
    public override string displayName { get; set; } = "Steady Aim";

    // IToolTip
    public string tooltipName { get; set; } = "Steady Aim";
    public string tooltip { get; set; } = "If this unit has not moved since last turn, significantly increase DMG, HIT, and CRIT. (Player Phase)";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        if (boundUnit.turnActive && boundUnit.moveAvailable) {
            mutAtt.damage += 3;
            mutAtt.hitRate += 50;
            mutAtt.critRate += 35;
            //
            mutAtt.AddMutator(this);
        }
    }
}