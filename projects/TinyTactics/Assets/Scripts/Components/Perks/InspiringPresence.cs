using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InspiringPresence : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Inspiring Presence";
    public string tooltip { get; set; } = "Grant 3 STR to adjacent allies.";

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
            if (gp == boundUnit.gridPosition || !boundUnit.battleMap.IsInBounds(gp)) continue;

            Unit? unit = boundUnit.unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() == boundUnit.GetType()) {
                // don't re-add the same buff a billion times
                if (unit.buffManager.HasBuffFromProvider<ConditionalStrengthBuff>(displayName))
                    continue;

                unit.buffManager.AddConditionalBuff<ConditionalStrengthBuff>(
                    displayName,
                    3,
                    () => unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) == 1
                ); 
            }
        }
    }
}