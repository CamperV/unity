using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class DeathAdder : Perk, IToolTip
{
    public override string displayName { get; set; } = "Razer DeathAdder";

    // IToolTip
    public string tooltipName { get; set; } = "Razer DeathAdder";
    public string tooltip { get; set; } = "+2 damage for each buff or debuff attached to the target.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // this will be changed...
    // right now we add damage for all statuses. In the future, do it for all debuffs only
    private void ConditionalAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        int accum = target.statusManager.AllStatuses().Count() * 2;

        if (accum > 0) {
            mutAtt.AddDamage(accum);
            mutAtt.AddMutator(this);
        }
    }
}