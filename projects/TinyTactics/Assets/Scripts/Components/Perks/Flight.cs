using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Flight : Perk, IToolTip
{
    public override string displayName { get; set; } = "Flight";

    // IToolTip
    public string tooltipName { get; set; } = "Flight";
    public string tooltip { get; set; } = "Fly over impassable and rough terrain. Cannot utilize terrain bonuses.";

    public override void OnAcquire() {
        boundUnit.unitPathfinder.moveThroughTerrainOverride = true;

        boundUnit.tags.Add("Flier");
    }

    public override void OnRemoval() {
        boundUnit.unitPathfinder.moveThroughTerrainOverride = false;

        boundUnit.tags.Remove("Flier");
    }
}