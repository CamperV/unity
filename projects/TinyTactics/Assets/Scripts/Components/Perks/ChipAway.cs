using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChipAway : Perk, IToolTip
{
    public override string displayName { get; set; } = "Chip Away";

    // IToolTip
    public string tooltipName { get; set; } = "Chip Away";
    public string tooltip { get; set; } = "When attacking or counterattacking, always deal at least 1 damage.";

    public override void OnAcquire() {
        boundUnit.OnFinalEngagementGeneration += ChipAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnFinalEngagementGeneration -= ChipAttack;
    }

    private void ChipAttack(ref MutableEngagementStats mutES) {
        if (mutES.damage < 1) {
            mutES.damage = 1;
            mutES.AddMutator(this);    
        }
    }
}