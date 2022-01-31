using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MaleficPresence : Perk, IToolTip
{
    public override string displayName { get; set; } = "Malefic Presence";

    // IToolTip
    public string tooltipName { get; set; } = "Malefic Presence";
    public string tooltip { get; set; } = "Inflict -10 AVOID to enemies within 2 spaces.";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += GrantBonusToAdjacentUnits;
    }

    public override void OnRemoval() {
        boundUnit.unitMap.NewBoardStateEvent -= GrantBonusToAdjacentUnits;
    }

    // if the unit has not moved since last turn, significantly buff attack
    // grant only to allies
    private void GrantBonusToAdjacentUnits() {
        foreach (Unit unit in boundUnit.EnemiesWithinRange(2)) {
            // don't re-add the same buff a billion times
            if (unit.statusManager.HasStatusFromProvider<ConditionalAvoidDebuff>(displayName))
                continue;

            unit.statusManager.AddConditionalBuff<ConditionalAvoidDebuff>(
                displayName,
                -10,
                () => boundUnit.gameObject.activeInHierarchy && unit.gridPosition.ManhattanDistance(boundUnit.gridPosition) <= 2
            ); 
        }
    }
}