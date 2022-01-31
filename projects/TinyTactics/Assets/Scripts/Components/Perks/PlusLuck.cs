using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlusLuck : Perk, IToolTip
{
    public override string displayName { get; set; } = "+10 Crit. Avoid";

    // IToolTip
    public string tooltipName { get; set; } = "+10 Crit. Avoid";
    public string tooltip { get; set; } = "Upon acquiring, +10 Crit. Avoid.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateLuck(boundUnit.unitStats._LUCK + 10);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateLuck(boundUnit.unitStats._LUCK - 10);
    }
}