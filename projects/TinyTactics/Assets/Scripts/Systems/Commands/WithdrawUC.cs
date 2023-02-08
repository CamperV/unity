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
    public CountdownStatus withdrawDebuff;

    public override void Activate(PlayerUnit thisUnit){}
    public override void Deactivate(PlayerUnit thisUnit){}
    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) => ExitSignal.ForceFinishTurn;
    public override void ActiveUpdate(PlayerUnit thisUnit){}
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) => ExitSignal.ForceFinishTurn;

    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        ProcFor(thisUnit);
        thisUnit.statusSystem.AddStatus(withdrawBuff, so_Status.CreateStatusProviderID(thisUnit, withdrawBuff));

        // also debuff POISE
        CountdownStatus clonedWithdrawDebuff = CountdownStatus.CloneWithValue(withdrawDebuff, -thisUnit.statSystem.CURRENT_POISE);
        thisUnit.statusSystem.AddStatus(clonedWithdrawDebuff, so_Status.CreateStatusProviderID(thisUnit, clonedWithdrawDebuff));

        return ExitSignal.ForceFinishTurn;
    }
}
