using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlusStr : Perk, IToolTip
{
    public override string displayName { get; set; } = "+2 STR";

    // IToolTip
    public string tooltipName { get; set; } = "+2 STR";
    public string tooltip { get; set; } = "Upon acquiring, +2 STRENGTH.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + 2);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - 2);
    }
}