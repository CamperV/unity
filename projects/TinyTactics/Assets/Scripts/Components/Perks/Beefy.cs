using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Beefy : Perk, IToolTip
{
    public override string displayName { get; set; } = "Beefy";

    // IToolTip
    public string tooltipName { get; set; } = "Beefy";
    public string tooltip { get; set; } = "Upon acquiring, increase VITALITY by STRENGTH.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateVitality(boundUnit.unitStats.VITALITY + boundUnit.unitStats.STRENGTH);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateVitality(boundUnit.unitStats.VITALITY - boundUnit.unitStats.STRENGTH);
    }
}