using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AntiFlier : WeaponPerk, IToolTip
{
    public override string displayName { get; set; } = "Weapon Effectiveness";

    // IToolTip
    public string tooltipName { get; set; } = "Weapon Effectiveness (Flier)";
    public string tooltip { get; set; } = "+100 CRIT when attacking Flying units.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnAttack += OffensiveAdv;
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnAttack -= OffensiveAdv;
    }

    private void OffensiveAdv(ref MutableAttack mutAtt, Unit target) {
        if (target.HasTagMatch("Flier")) {
            mutAtt.critRate += 100;
            //
            mutAtt.AddMutator(this);
        }
    }
}