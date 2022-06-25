using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class KunaiDebuff : WeaponPerk, IToolTip
{
    public override string displayName { get; set; } = "Kunai Debuff";
    
    public string tooltipName { get; set; } = "Kunai Debuff";
    public string tooltip { get; set; } = "On hit, -2 DEF, -2 REF.";

    public AudioFXBundle audioFXBundle;

    public override void OnEquip() {
        // boundWeapon.boundUnit.OnHit += ApplyDebuff;
    }

    public override void OnUnequip() {
        // boundWeapon.boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        if (target.gameObject.activeInHierarchy) {
            // queue the sound and animation for after it is done animating the Hurt animation
            target.spriteAnimator.QueueAction(
                () => target.TriggerDebuffAnimation(audioFXBundle.RandomClip(), "DEF", "REF")
            );
            
            target.statusManager.AddValuedStatus<DefenseDebuff>(displayName, -2);
            target.statusManager.AddValuedStatus<ReflexDebuff>(displayName, -2);
        }
    }
}