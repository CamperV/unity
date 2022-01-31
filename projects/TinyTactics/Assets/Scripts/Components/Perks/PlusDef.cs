using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlusDef : Perk, IToolTip
{
    public override string displayName { get; set; } = "+2 DEF";

    // IToolTip
    public string tooltipName { get; set; } = "+2 DEF";
    public string tooltip { get; set; } = "Upon acquiring, +2 DEFENSE.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + 2);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - 2);
    }
}