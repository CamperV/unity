using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HealingPresence : Perk, IToolTip
{
    public override string displayName { get; set; } = "Healing Presence";

    // IToolTip
    public string tooltipName { get; set; } = "Healing Presence";
    public string tooltip { get; set; } = "On start of turn, heal adjacent allies by 25% of their Max HP.";

    public override void OnAcquire() {
        boundUnit.OnStartTurn += HealAdjacentUnits;
    }

    public override void OnRemoval() {
        boundUnit.OnStartTurn -= HealAdjacentUnits;
    }

    // grant only to allies
    private void HealAdjacentUnits(Unit _) {
        foreach (Unit unit in boundUnit.AlliesWithinRange(1)) {
            int healAmount = (int)(0.25f * unit.unitStats.VITALITY);
            unit.HealAmount(healAmount);
        }
    }
}