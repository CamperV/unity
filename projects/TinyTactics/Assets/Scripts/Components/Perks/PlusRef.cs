using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlusRef : Perk, IToolTip
{
    public override string displayName { get; set; } = "+4 REF";

    // IToolTip
    public string tooltipName { get; set; } = "+4 REF";
    public string tooltip { get; set; } = "Upon acquiring, +4 REFLEX.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX + 4);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX - 4);
    }
}