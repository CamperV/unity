using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/WithdrawUC")]
public class WithdrawUC : UnitCommand
{
    public AudioFXBundle audioFXBundle;
    public CountdownStatus withdrawBuff;

    public override void Activate(PlayerUnit thisUnit){}
    public override void Deactivate(PlayerUnit thisUnit){}
    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) => ExitSignal.ForceFinishTurn;
    public override void ActiveUpdate(PlayerUnit thisUnit){}
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) => ExitSignal.ForceFinishTurn;

    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        // queue the sound and animation for after it is done animating the Hurt animation
        thisUnit.spriteAnimator.QueueAction(
            () => thisUnit.TriggerBuffAnimation(audioFXBundle.RandomClip(), "DEF")
        );
        thisUnit.statusSystem.AddStatus(withdrawBuff, so_Status.CreateStatusProviderID(thisUnit, withdrawBuff));
        thisUnit.statSystem.UpdatePoise(0);

        return ExitSignal.ForceFinishTurn;
    }
}
