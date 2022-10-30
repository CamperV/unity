using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/WaitUC")]
public class WaitUC : UnitCommand
{
    public override void Activate(PlayerUnit thisUnit){}
    public override void Deactivate(PlayerUnit thisUnit){}
    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) => ExitSignal.ForceFinishTurn;
    public override void ActiveUpdate(PlayerUnit thisUnit){}
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) => ExitSignal.ForceFinishTurn;

    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        thisUnit.personalAudioFX.PlaySpecialInteractFX();
        thisUnit.FireOnWaitEvent();
        return ExitSignal.ForceFinishTurn;
    }
}
