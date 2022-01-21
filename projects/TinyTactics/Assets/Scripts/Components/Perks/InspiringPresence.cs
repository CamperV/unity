using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InspiringPresence : Perk, IToolTip
{
    public override string displayName { get; set; } = "Inspiring Presence";

    // IToolTip
    public string tooltipName { get; set; } = "Inspiring Presence";
    public string tooltip { get; set; } = "Grant 3 STR to adjacent allies.";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += GrantBonusToAdjacentUnits;
    }

    public override void OnRemoval() {
        boundUnit.unitMap.NewBoardStateEvent -= GrantBonusToAdjacentUnits;
    }

    // if the unit has not moved since last turn, significantly buff attack
    // grant only to allies
    private void GrantBonusToAdjacentUnits() {
        foreach (Unit unit in boundUnit.AlliesWithinRange(1)) {
            // don't re-add the same buff a billion times
            if (unit.statusManager.HasStatusFromProvider<ConditionalStrengthBuff>(displayName))
                continue;

            unit.statusManager.AddConditionalBuff<ConditionalStrengthBuff>(
                displayName,
                3,
                () => boundUnit.gameObject.activeInHierarchy && unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) == 1
            ); 
        }
    }
}