using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Charge : Perk, IToolTip
{
    public override string displayName { get; set; } = "Charge";

    // IToolTip
    public string tooltipName { get; set; } = "Charge";
    public string tooltip { get; set; } = "Gain +1 DMG per space moved this turn.";

    public override void OnAcquire() {
        boundUnit.OnMove += GainDamageBuffPerMove;

        //
        boundUnit.statusManager.movementBuffProviders.Add("Charge");
    }

    public override void OnRemoval() {
        boundUnit.OnMove -= GainDamageBuffPerMove;
        boundUnit.statusManager.movementBuffProviders.Remove("Charge");
    }

    // adds a damage buff per square moved this turn
    private void GainDamageBuffPerMove(Path<GridPosition> pathTaken) {
        for (int i = 0; i < pathTaken.Count-1; i++) {
            boundUnit.statusManager.AddValuedStatus<OneTimeDamageBuff>("Charge", 1);
        }
    }
}