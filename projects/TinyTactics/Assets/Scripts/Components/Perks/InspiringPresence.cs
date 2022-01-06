using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InspiringPresence : Perk
{
    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += GrantBonusToAdjacentUnits;
        //
        displayName = "Inspiring Presence";
    }

    public override void OnRemoval() {
        boundUnit.unitMap.NewBoardStateEvent -= GrantBonusToAdjacentUnits;
    }

    // if the unit has not moved since last turn, significantly buff attack
    // grant only to allies
    private void GrantBonusToAdjacentUnits() {

        foreach (GridPosition gp in boundUnit.gridPosition.Radiate(1)) {
            if (gp == boundUnit.gridPosition) continue;

            Unit? unit = boundUnit.unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() == boundUnit.GetType()) {
                unit.buffManager.AddConditionalBuff<ConditionalStrengthBuff>(
                    "Inspiring Presence",
                    3,
                    () => unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) == 1
                ); 
            }
        }
    }
}