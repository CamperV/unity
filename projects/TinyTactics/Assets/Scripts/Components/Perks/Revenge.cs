using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Revenge : Perk, IToolTip
{
    public override string displayName { get; set; } = "Revenge";

    // IToolTip
    public string tooltipName { get; set; } = "Revenge";
    public string tooltip { get; set; } = "After being hit, gain a +2 STRENGTH buff.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        // boundUnit.OnHurt += GainDamageBuff;
    }

    public override void OnRemoval() {
        // boundUnit.OnHurt -= GainDamageBuff;
    }

    private void GainDamageBuff() {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;

        // queue the sound and animation for after it is done animating the Hurt animation
        boundUnit.spriteAnimator.QueueAction(
            () => boundUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "STR")
        );
        //
        boundUnit.statusManager.AddValuedStatus<StrengthBuff>("Revenge", 2);
    }
}