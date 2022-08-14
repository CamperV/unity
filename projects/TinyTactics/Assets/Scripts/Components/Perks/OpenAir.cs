using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OpenAir : Perk, IToolTip
{
    public override string displayName { get; set; } = "Open Air (does not work)";

    // IToolTip
    public string tooltipName { get; set; } = "Open Air (does not work)";
    public string tooltip { get; set; } = "+2 damage when not adjacent to any walls.";

    public override void OnAcquire() {
        boundUnit.OnAttack += ConditionalAttack;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= ConditionalAttack;
    }

    // check all walls every time you create an attack
    private void ConditionalAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        bool wallsAdjacent = false;

        foreach (TerrainTile tt in boundUnit.TerrainWithinRange(1)) {
            if (tt.displayName == "Wall") {
                wallsAdjacent = true;
                break;
            }
        }

        if (wallsAdjacent == false) {
            mutAtt.AddBonusDamage(2);
            mutAtt.AddMutator(this);
        }
    }
}