using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class CripplingStrike : Perk, IToolTip
{
    public string tooltipName { get; set; } = "Crippling Strike";
    public string tooltip { get; set; } = "On hit, -50% MOV debuff until end of next turn.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnHit += ApplyDebuff;
        
        displayName = "Crippling Strike";
    }

    public override void OnRemoval() {
        boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        if (target.gameObject.activeInHierarchy) {
            // queue the sound and animation for after it is done animating the Hurt animation
            target.spriteAnimator.QueueAction(
                () => target.TriggerDebuffAnimation(audioFXBundle.RandomClip())
            );
            
            float halfMove = target.unitStats.MOVE/2f;
            int roundedValue = (int)Mathf.Floor(halfMove);
            target.statusManager.AddValuedStatus<MoveDebuff>(displayName, roundedValue, 1);
        }

    }
}