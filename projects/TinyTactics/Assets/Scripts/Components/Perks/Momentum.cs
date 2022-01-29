using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Momentum : Perk, IToolTip
{
    public override string displayName { get; set; } = "Momentum";

    // IToolTip
    public string tooltipName { get; set; } = "Momentum";
    public string tooltip { get; set; } = "After moving the max movement range, gain a +2 MOVE buff.";

    public override void OnAcquire() {
        boundUnit.OnMove += ConditionalMoveBuff;
        boundUnit.statusManager.movementBuffProviders.Add($"Momentum");
    }

    public override void OnRemoval() {
        boundUnit.OnMove -= ConditionalMoveBuff;
        boundUnit.statusManager.movementBuffProviders.Remove("Momentum");
    }

    // adds a damage buff per square moved this turn
    private void ConditionalMoveBuff(Path<GridPosition> pathTaken) {
        if ((pathTaken.Count-1) == boundUnit.unitStats.MOVE) {

            AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;
            boundUnit.spriteAnimator.QueueAction(
                () => boundUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "MOV")
            );

            boundUnit.statusManager.AddValuedStatus<MoveBuff>("Momentum", 2);
        }
    }
}