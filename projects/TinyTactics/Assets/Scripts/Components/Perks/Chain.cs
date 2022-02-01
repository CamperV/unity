using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Chain : Perk, IToolTip
{
    public override string displayName { get; set; } = $"Chain";

    // IToolTip
    public string tooltipName { get; set; } = "Chain";
    public string tooltip { get; set; } = "On hit, +1 non-expiring STR. On miss, lose all accumulated bonus STR.";

    public override void OnAcquire() {
        boundUnit.OnHit += GainDamageBuffPerHit;
        boundUnit.OnMiss += LoseDamageBuffs;
    }

    public override void OnRemoval() {
        boundUnit.OnHit -= GainDamageBuffPerHit;
        boundUnit.OnMiss -= LoseDamageBuffs;
    }

    private void GainDamageBuffPerHit(Unit target) {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;
        boundUnit.spriteAnimator.QueueAction(
            () => boundUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "STR")
        );

        boundUnit.statusManager.AddValuedStatus<ChainBuff>("Chain", 1);
    }

    private void LoseDamageBuffs() {
        if (boundUnit.statusManager.HasStatusFromProvider<ChainBuff>("Chain")) {
            AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/DebuffAudioFXBundle") as AudioFXBundle;
            boundUnit.spriteAnimator.QueueAction(
                () => boundUnit.TriggerDebuffAnimation(loadedBundle.RandomClip(), "STR")
            );

            boundUnit.statusManager.RemoveAllStatusFromProvider("Chain");
        }
    }
}