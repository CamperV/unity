using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AfterImage : Perk, IToolTip
{
    // IToolTip
    public string tooltipName { get; set; } = "Afterimage";
    public string tooltip { get; set; } = "After avoiding an enemy attack, move again. (Player Phase)";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnAvoid += RefreshMovement;
        //
        displayName = "Afterimage";
    }

    public override void OnRemoval() {
        boundUnit.OnAvoid -= RefreshMovement;
    }

    private void RefreshMovement() {
        if (boundUnit.turnActive) {
            boundUnit.spriteAnimator.QueueAction(
                () => boundUnit.TriggerBuffAnimation(audioFXBundle.RandomClip())
            );

            boundUnit.moveAvailable = true;
        }
    }
}