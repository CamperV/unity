using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Patience : Perk, IToolTip
{
    public override string displayName { get; set; } = "Patience";

    // IToolTip
    public string tooltipName { get; set; } = "Patience";
    public string tooltip { get; set; } = "On wait, gain a one-time +4 DEFENSE buff.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnWait += GainDefenseBuff;
    }

    public override void OnRemoval() {
        boundUnit.OnWait -= GainDefenseBuff;
    }

    private void GainDefenseBuff() {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;

        // queue the sound and animation for after it is done animating the Hurt animation
        boundUnit.spriteAnimator.QueueAction(
            () => boundUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "DEF")
        );
        //
        boundUnit.statusManager.AddValuedStatus<OneTimeDefenseBuff>("Patience", 4);
    }
}