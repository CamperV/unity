using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Nimble : Perk, IToolTip
{
    public override string displayName { get; set; } = "Nimble";

    // IToolTip
    public string tooltipName { get; set; } = "Nimble";
    public string tooltip { get; set; } = "Move through enemy-occupied spaces.";

    public override void OnAcquire() {
        boundUnit.unitPathfinder.moveThroughEnemiesOverride = true;
        //
        boundUnit.OnMove += GainMultistrikePerPass;
        boundUnit.statusManager.movementBuffProviders.Add("Nimble");
    }

    public override void OnRemoval() {
        boundUnit.unitPathfinder.moveThroughEnemiesOverride = false;
        //
        boundUnit.OnMove -= GainMultistrikePerPass;
        boundUnit.statusManager.movementBuffProviders.Remove("Nimble");
    }

    private void GainMultistrikePerPass(Path<GridPosition> pathTaken) {
        int enemyCount = 0;
        foreach (GridPosition gp in pathTaken.Unwind()) {
            if (EnemyAt(gp)) enemyCount++;
        }

        boundUnit.statusManager.AddValuedStatus<OneTimeMultistrikeBuff>("Nimble", enemyCount);
    }

    private bool EnemyAt(GridPosition gp) {
        if (!boundUnit.battleMap.IsInBounds(gp)) return false;

        Unit unit = boundUnit.unitMap.UnitAt(gp);
        return unit != null && unit.GetType() == typeof(EnemyUnit);
    }
}