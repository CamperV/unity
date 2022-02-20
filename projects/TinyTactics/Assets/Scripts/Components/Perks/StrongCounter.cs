using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StrongCounter : Perk, IToolTip
{
    public override string displayName { get; set; } = "Strong Counter";

    // IToolTip
    public string tooltipName { get; set; } = "Strong Counter";
    public string tooltip { get; set; } = "When counterattacking, increase DMG.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // if the unit has not moved since last turn, significantly buff attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        if (boundUnit.moveAvailable) {
            mutAtt.AddDamage(3);
            //
            mutAtt.AddMutator(this);
        }
    }
}