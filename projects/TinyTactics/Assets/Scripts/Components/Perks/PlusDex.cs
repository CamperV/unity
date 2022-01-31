using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlusDex : Perk, IToolTip
{
    public override string displayName { get; set; } = "+4 DEX";

    // IToolTip
    public string tooltipName { get; set; } = "+4 DEX";
    public string tooltip { get; set; } = "Upon acquiring, +4 DEXTERITY.";

    public override void OnAcquire() {
        boundUnit.unitStats.UpdateDexterity(boundUnit.unitStats.DEXTERITY + 4);
    }

    public override void OnRemoval() {
        boundUnit.unitStats.UpdateDexterity(boundUnit.unitStats.DEXTERITY - 4);
    }
}