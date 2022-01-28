using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class CripplingStrike : Perk, IToolTip
{
    public override string displayName { get; set; } = "Crippling Strike";

    public string tooltipName { get; set; } = "Crippling Strike";
    public string tooltip { get; set; } = "On hit, reduce target's MOVE to 0 until end of turn.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnHit += ApplyDebuff;
    }

    public override void OnRemoval() {
        boundUnit.OnHit -= ApplyDebuff;
    }

    private void ApplyDebuff(Unit target) {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/DebuffAudioFXBundle") as AudioFXBundle;

        // queue the sound and animation for after it is done animating the Hurt animation
        target.spriteAnimator.QueueAction(
            () => target.TriggerDebuffAnimation(loadedBundle.RandomClip(), "MOV")
        );
        
        target.statusManager.AddValuedStatus<OneTimeMoveDebuff>(displayName, -target.unitStats.MOVE);
    }
}