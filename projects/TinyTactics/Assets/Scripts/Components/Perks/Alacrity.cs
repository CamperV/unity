using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Alacrity : Perk, IToolTip
{
    public override string displayName { get; set; } = "Alacrity";

    // IToolTip
    public string tooltipName { get; set; } = "Alacrity";
    public string tooltip { get; set; } = "On start of turn, grant all adjacent allies +1 MOVE.";

    public AudioFXBundle audioFXBundle;

    public override void OnAcquire() {
        boundUnit.OnStartTurn += GrantMoveToAdjacentUnits;
    }

    public override void OnRemoval() {
        boundUnit.OnStartTurn -= GrantMoveToAdjacentUnits;
    }

    // grant only to allies
    private void GrantMoveToAdjacentUnits(Unit _) {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;
        
        foreach (Unit unit in boundUnit.AlliesWithinRange(1)) {
            unit.spriteAnimator.QueueAction(
                () => unit.TriggerBuffAnimation(loadedBundle.RandomClip(), "MOV")
            );
            unit.statusManager.AddValuedStatus<MoveBuff>("Alacrity", 1);
        }
    }
}