using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Nimble : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Nimble";
    public string tooltip { get; set; } = "Move through enemy-occupied spaces.";

    public override void OnAcquire() {
        boundUnit.unitPathfinder.moveThroughEnemiesOverride = true;
        //
        displayName = "Nimble";
    }

    public override void OnRemoval() {
        boundUnit.unitPathfinder.moveThroughEnemiesOverride = false;
    }
}