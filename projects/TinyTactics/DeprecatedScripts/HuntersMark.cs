using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class HuntersMark : Perk, IToolTip
{
    public override string displayName { get; set; } = "Hunter's Mark";

    public string tooltipName { get; set; } = "Hunter's Mark";
    public string tooltip { get; set; } = "On hit, grant all allies +25 HIT against target.";

    public override void OnAcquire() {
        boundUnit.OnHit += GrantMarkBonusAgainst;
    }

    public override void OnRemoval() {
        boundUnit.OnHit -= GrantMarkBonusAgainst;
    }

    private void GrantMarkBonusAgainst(Unit target) {
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/DebuffAudioFXBundle") as AudioFXBundle;

        // queue the sound and animation for after it is done animating the Hurt animation
        target.spriteAnimator.QueueAction(
            () => target.TriggerDebuffAnimation(loadedBundle.RandomClip(), "")
        );
        
        //
        UIManager.inst.combatLog.AddEntry($"BLUE@[Hunter's Mark] applied YELLOW@[+25 HIT] to allies against {target.logTag}@[{target.displayName}].");

        // instead of debuffing one unit... apply a Buff to all allies that only works against this unit
        foreach (Unit unit in boundUnit.Allies()) {
            // don't re-add the same buff a billion times
            if (unit.statusManager.HasStatusFromProvider<MarkBuff>(displayName))
                continue;

            unit.statusManager.AddCoupledBuff<MarkBuff>(
                displayName,
                25,
                target
            ); 
        }
    }

}