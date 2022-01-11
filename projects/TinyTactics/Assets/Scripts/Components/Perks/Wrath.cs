using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Wrath : Perk, IToolTip
{
    public override string displayName { get; set; } = "Wrath";

    // IToolTip
    public string tooltipName { get; set; } = "Wrath";
    public string tooltip { get; set; } = "+25 CRIT when under 50% HP.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        if ((float)boundUnit.unitStats._CURRENT_HP <= boundUnit.unitStats.VITALITY/2) {
            mutAtt.critRate += 25;
            mutAtt.AddMutator(this);
        }
    }
}