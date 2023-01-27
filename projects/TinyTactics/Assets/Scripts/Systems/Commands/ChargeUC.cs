using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/ChargeUC")]
public class ChargeUC : MoveUC
{
    public ImmediateEngagementStatus chargeBuff;

    protected override void OnMoveEffects(Unit thisUnit, Path<GridPosition> pathTaken) {
        ImmediateEngagementStatus clonedChargeBuff = ImmediateEngagementStatus.CloneWithValue(chargeBuff, chargeBuff.value * (pathTaken.Count-1));
        thisUnit.statusSystem.AddStatus(clonedChargeBuff, so_Status.CreateStatusProviderID(thisUnit, clonedChargeBuff));
    }
}
