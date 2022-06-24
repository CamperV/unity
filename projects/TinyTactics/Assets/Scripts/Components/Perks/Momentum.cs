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

    private bool trigger = false;

    public override void OnAcquire() {
        // boundUnit.OnMove += CheckMoveCondition;
        // boundUnit.OnStartTurn += ConditionalMoveBuff;
    }

    public override void OnRemoval() {
        // boundUnit.OnMove -= CheckMoveCondition;
        // boundUnit.OnStartTurn -= ConditionalMoveBuff;
    }

    // adds a damage buff per square moved this turn
    private void CheckMoveCondition(Path<GridPosition> pathTaken) {
        trigger = (pathTaken.Count-1) == boundUnit.unitStats.MOVE;
    }

    private void ConditionalMoveBuff(Unit target) {
        // this trigger is set last turn
        if (trigger) {
            AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;
            target.spriteAnimator.QueueAction(
                () => target.TriggerBuffAnimation(loadedBundle.RandomClip(), "MOV")
            );

            target.statusManager.AddValuedStatus<MoveBuff>("Momentum", 2);
        }

        // reset the trigger just in case
        trigger = false;
    }
}