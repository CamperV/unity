using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class WeaponDebuff : WeaponPerk
{
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