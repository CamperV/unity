using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ShieldWall : Perk, IToolTip
{
    public override string displayName { get; set; } = "Shield Wall";

    // IToolTip
    public string tooltipName { get; set; } = "Shield Wall";
    public string tooltip { get; set; } = "Grant 3 DEF to adjacent allies.";

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
            if (unit.statusManager.HasStatusFromProvider<ConditionalDefenseBuff>(displayName))
                continue;

            unit.statusManager.AddConditionalBuff<ConditionalDefenseBuff>(
                displayName,
                3,
                () => unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) == 1
            ); 
        }
    }
}