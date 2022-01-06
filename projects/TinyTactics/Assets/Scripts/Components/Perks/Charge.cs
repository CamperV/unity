using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Charge : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Charge";
    public string tooltip { get; set; } = "Gain +1 DMG per space moved this turn.";

    public override void OnAcquire() {
        boundUnit.OnMove += GainDamageBuffPerMove;

        //
        displayName = "Charge";
        boundUnit.buffManager.movementBuffProviders.Add("Charge");
    }

    public override void OnRemoval() {
        boundUnit.OnMove -= GainDamageBuffPerMove;
        boundUnit.buffManager.movementBuffProviders.Remove("Charge");
    }

    // adds a damage buff per square moved this turn
    private void GainDamageBuffPerMove(Path<GridPosition> pathTaken) {
        for (int i = 0; i < pathTaken.Count-1; i++) {
            boundUnit.buffManager.AddValueBuff<DamageBuff>("Charge", 1, 1);
        }
    }
}