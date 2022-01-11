using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GuidingPresence : Perk, IToolTip
{
    public override string displayName { get; set; } = "Guiding Presence";

    // IToolTip
    public string tooltipName { get; set; } = "Guiding Presence";
    public string tooltip { get; set; } = "Grant +5 DEX to allies within 2 spaces.";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += GrantBonusToAdjacentUnits;
    }

    public override void OnRemoval() {
        boundUnit.unitMap.NewBoardStateEvent -= GrantBonusToAdjacentUnits;
    }

    // if the unit has not moved since last turn, significantly buff attack
    // grant only to allies
    private void GrantBonusToAdjacentUnits() {
        foreach (Unit unit in boundUnit.AlliesWithinRange(2)) {
            // don't re-add the same buff a billion times
            if (unit.statusManager.HasStatusFromProvider<ConditionalDexterityBuff>(displayName))
                continue;

            unit.statusManager.AddConditionalBuff<ConditionalDexterityBuff>(
                displayName,
                5,
                () => unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) <= 2
            ); 
        }
    }
}