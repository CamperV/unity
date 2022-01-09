using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Revenge : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Revenge";
    public string tooltip { get; set; } = "After being attacked, gain +2 DMG (stacking) until the end of next turn.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnHurt += GainDamageBuff;
        //
        displayName = "Revenge";
    }

    public override void OnRemoval() {
        boundUnit.OnHurt -= GainDamageBuff;
    }

    private void GainDamageBuff() {
        // queue the sound and animation for after it is done animating the Hurt animation
        boundUnit.spriteAnimator.QueueAction(
            () => boundUnit.TriggerBuffAnimation(audioFXBundle.RandomClip())
        );
        //
        boundUnit.statusManager.AddValuedStatus<DamageBuff>("Revenge", 2, 1);
    }
}