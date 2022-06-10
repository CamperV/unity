using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/FullDefenseUC")]
public class FullDefenseUC : UnitCommand
{
    public override void Activate(PlayerUnit thisUnit){}
    public override void Deactivate(PlayerUnit thisUnit){}
    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) => ExitSignal.ForceFinishTurn;
    public override void ActiveUpdate(PlayerUnit thisUnit){}
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) => ExitSignal.ForceFinishTurn;

    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        GainDefenseBuff(thisUnit);
        thisUnit.counterAttackAvailable = false; // until it starts its turn again
        return ExitSignal.ForceFinishTurn;
    }

    private void GainDefenseBuff(PlayerUnit thisUnit) {
        // queue the sound and animation for after it is done animating the Hurt animation
        AudioFXBundle loadedBundle = Resources.Load<AudioFXBundle>("ScriptableObjects/AudioFXBundles/BuffAudioFXBundle") as AudioFXBundle;
        thisUnit.spriteAnimator.QueueAction(
            () => thisUnit.TriggerBuffAnimation(loadedBundle.RandomClip(), "DEF")
        );
        thisUnit.statusManager.AddValuedStatus<OneTimeDefenseBuff>(this.name, 3);
    }
}
