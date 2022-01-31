using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HealingTide : Perk, IToolTip
{
    public override string displayName { get; set; } = "Healing Tide";

    // IToolTip
    public string tooltipName { get; set; } = "Healing Tide";
    public string tooltip { get; set; } = "On wait, heal allies within 2 spaces by 10% of their Max HP.";

    public override void OnAcquire() {
        boundUnit.OnWait += HealAllies;
    }

    public override void OnRemoval() {
        boundUnit.OnWait -= HealAllies;
    }

    private void HealAllies() {
        foreach (Unit unit in boundUnit.AlliesWithinRange(2)) {
            int healAmount = (int)(0.1f * unit.unitStats.VITALITY);
            unit.HealAmount(healAmount);
        }
    }
}