using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class WeaponDebuff : WeaponPerk, IToolTip
{
    public string tooltipName { get; set; } = "Weapon Debuff (Reflex)";
    public string tooltip { get; set; } = "On hit, -5 Reflex until end of next turn.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnHit += ApplyDebuff;
        
        displayName = "Weapon Debuff (Reflex)";
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        target.buffManager.AddValueBuff<ReflexDebuff>(displayName, 5, 1);
    }
}