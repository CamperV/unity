using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReducibleDefenseBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Defense ({provider})";
    public override string affectedStat => "DEF";

    public override void OnAcquire() {
        boundUnit.OnHurt += _TickExpire;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnHurt -= _TickExpire;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - modifierValue);
    }

    // can't use an anonymous function here, because it will never be removed via "-="
    private void _TickExpire() {
        // queue the sound and animation for after it is done animating the Hurt animation
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/DebuffAudioFXBundle") as AudioFXBundle;

        boundUnit.spriteAnimator.QueueAction(
            () => boundUnit.TriggerDebuffAnimation(loadedBundle.RandomClip(), "DEF")
        );
        TickExpire(boundUnit);
    }
}