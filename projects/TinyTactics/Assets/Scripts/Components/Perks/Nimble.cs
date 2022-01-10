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
    }

    public override void OnRemoval() {
        boundUnit.unitPathfinder.moveThroughEnemiesOverride = false;
    }
}