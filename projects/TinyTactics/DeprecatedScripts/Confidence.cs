using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Confidence : Perk, IToolTip
{
    public override string displayName { get; set; } = "Confidence";

    // IToolTip
    public string tooltipName { get; set; } = "Confidence";
    public string tooltip { get; set; } = "+15 HIT, +15 CRIT when there are no allies within 2 spaces.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        int closeAllies = boundUnit.AlliesWithinRange(2).ToList().Count;

        if (closeAllies == 0) {
            mutAtt.hitRate += 15;
            mutAtt.critRate += 15;
            //
            mutAtt.AddMutator(this);
        }
    }
}