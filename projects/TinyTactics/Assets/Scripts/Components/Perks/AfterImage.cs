using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AfterImage : Perk, IToolTip
{
    public override string displayName { get; set; } = "Afterimage";

    // IToolTip
    public string tooltipName { get; set; } = "Afterimage";
    public string tooltip { get; set; } = "After avoiding an enemy attack, move again. (Player Phase)";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnAvoid += RefreshMovement;
    }

    public override void OnRemoval() {
        boundUnit.OnAvoid -= RefreshMovement;
    }

    private void RefreshMovement() {
        if (boundUnit.turnActive) {
            AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;

            boundUnit.spriteAnimator.QueueAction(
                () => boundUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "MOV")
            );

            boundUnit.moveAvailable = true;
        }
    }
}