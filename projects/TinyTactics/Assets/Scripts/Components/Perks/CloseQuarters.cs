using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CloseQuarters : Perk, IToolTip
{
    public override string displayName { get; set; } = "Close Quarters";

    // IToolTip
    public string tooltipName { get; set; } = "Close Quarters";
    public string tooltip { get; set; } = "+2 damage when adjacent to a wall.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // check all walls every time you create an attack
    private void ConditionalAttack(ref MutableAttack mutAtt, Unit target) {
        bool wallsAdjacent = false;

        foreach (TerrainTile tt in boundUnit.TerrainWithinRange(1)) {
            if (tt.displayName == "Wall") {
                wallsAdjacent = true;
                break;
            }
        }

        if (wallsAdjacent == true) {
            mutAtt.AddDamage(2);
            mutAtt.AddMutator(this);
        }
    }
}