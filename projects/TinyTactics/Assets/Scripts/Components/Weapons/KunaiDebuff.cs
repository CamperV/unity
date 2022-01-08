using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class KunaiDebuff : WeaponPerk, IToolTip
{
    public string tooltipName { get; set; } = "Kunai Debuff";
    public string tooltip { get; set; } = "On hit, -3 DEF, -5 REF until end of next turn.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnHit += ApplyDebuff;
        
        displayName = "Kunai Debuff";
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        target.statusManager.AddValuedStatus<DefenseDebuff>(displayName, 3, 2);
        target.statusManager.AddValuedStatus<ReflexDebuff>(displayName, 5, 2);
    }
}