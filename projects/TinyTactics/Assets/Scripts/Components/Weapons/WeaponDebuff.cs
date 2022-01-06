using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class WeaponDebuff : WeaponPerk, IToolTip
{
    public string tooltipName { get; set; } = "Debuff Reflex";
    public string tooltip { get; set; } = "On hit, -5 Reflex until end of next turn.";

    public override void OnEquip() {
        boundWeapon.boundUnit.OnHit += ApplyDebuff;
        
        displayName = "Weapon Debuff";
    }

    public override void OnUnequip() {
        boundWeapon.boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        target.buffManager.AddValueBuff<ReflexDebuff>("Weapon Debuff", 5, 1);
    }
}