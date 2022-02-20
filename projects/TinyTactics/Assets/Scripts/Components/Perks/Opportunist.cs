using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Opportunist : Perk, IToolTip
{
    public override string displayName { get; set; } = "Opportunist";

    // IToolTip
    public string tooltipName { get; set; } = "Opportunist";
    public string tooltip => "Deal +5 damage when the target cannot counterattack.";

    public override void OnAcquire() {
        boundUnit.OnAttack += OffensiveAdv;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= OffensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        if (!Engagement.CounterAttackPossible(boundUnit, target)) {
            mutAtt.AddDamage(5);
            mutAtt.AddMutator(this);
        }
    }
}