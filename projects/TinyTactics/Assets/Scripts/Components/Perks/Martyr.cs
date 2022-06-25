using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Martyr : Perk, IToolTip
{
    public override string displayName { get; set; } = "Martyr";

    // IToolTip
    public string tooltipName { get; set; } = "Martyr";
    public string tooltip { get; set; } = "After being attacked, heal allies within 2 spaces for 25% of their VITALITY.";

    public override void OnAcquire() {
        // boundUnit.OnHurt += HealAdjacent;
    }

    public override void OnRemoval() {
        // boundUnit.OnHurt -= HealAdjacent;
    }

    private void HealAdjacent() {
        foreach(Unit unit in boundUnit.AlliesWithinRange(2)) {
            unit.HealAmount((int)(0.25f*unit.unitStats.VITALITY));
        }
    }
}