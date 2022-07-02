using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Longshot : Perk, IToolTip
{
    public override string displayName { get; set; } = "Longshot";

    // IToolTip
    public string tooltipName { get; set; } = "Longshot";
    public string tooltip { get; set; } = "When using Missile weapons, +1 max attack range.";

    public override void OnAcquire() {
        // if (boundUnit.EquippedWeapon.HasTagMatch("Missile")) {
        //     boundUnit.EquippedWeapon.weaponStats.MAX_RANGE += 1;
        // }
    }

    public override void OnRemoval() {
        // if (boundUnit.EquippedWeapon.HasTagMatch("Missile")) {
        //     boundUnit.EquippedWeapon.weaponStats.MAX_RANGE -= 1;
        // }
    }
}