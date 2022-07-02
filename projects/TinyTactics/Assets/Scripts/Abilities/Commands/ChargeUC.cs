using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/ChargeUC")]
public class ChargeUC : MoveUC
{
    public ImmediateEngagementStatus chargeBuff;

    protected override void ExecuteAdditionalOnMove(PlayerUnit thisUnit, Path<GridPosition> pathTaken) {
        ImmediateEngagementStatus clonedChargeBuff = ImmediateEngagementStatus.CloneWithValue(chargeBuff, pathTaken.Count-1);
        thisUnit.statusSystem.AddStatus(clonedChargeBuff, so_Status.CreateStatusProviderID(thisUnit, clonedChargeBuff));
    }

    // this ignores auxiliaryInteract, so you can't accidentally immediately Wait
    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        thisUnit.ClaimReservation();
        return ExitSignal.ContinueTurn;
    }
}
