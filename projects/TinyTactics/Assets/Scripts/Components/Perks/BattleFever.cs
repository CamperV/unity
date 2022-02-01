using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BattleFever : Perk, IToolTip
{
    public override string displayName { get; set; } = "Battle Fever";

    // IToolTip
    public string tooltipName { get; set; } = "Battle Fever";
    public string tooltip { get; set; } = "+1 CRIT for each missing point of HP.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        int diff = boundUnit.unitStats.VITALITY - boundUnit.unitStats._CURRENT_HP;
        if (diff > 0) {
            mutAtt.critRate += diff;
            mutAtt.AddMutator(this);
        }
    }
}