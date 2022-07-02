using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/NimbleUC")]
public class NimbleUC : MoveUC
{
    public ImmediateValueStatus nimbleBuff;

    public override void Activate(PlayerUnit thisUnit) {
        // NimbleUC
        thisUnit.unitPathfinder.moveThroughEnemiesOverride = true;
        // NimbleUC
        thisUnit.UpdateThreatRange();

        // re-calc move range, and display it
        Utils.DelegateLateFrameTo(thisUnit,  () => DisplayMoveRange(thisUnit));
        UIManager.inst.EnableUnitDetail(thisUnit);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlightTiles();
        thisUnit.battleMap.ResetHighlight();
        thisUnit.battleMap.ClearDisplayPath();
        //
        UIManager.inst.DisableUnitDetail();
        
        thisUnit.unitPathfinder.moveThroughEnemiesOverride = false;
    }

    protected override void ExecuteAdditionalOnMove(PlayerUnit thisUnit, Path<GridPosition> pathTaken) {
        int passedEnemies = 0;
        foreach (GridPosition gp in pathTaken.Unwind()) {
            if (EnemyAt(thisUnit, gp)) passedEnemies++;
        }
    
        ImmediateValueStatus clonedNimbleBuff = ImmediateValueStatus.CloneWithValue(nimbleBuff, passedEnemies);
        thisUnit.statusSystem.AddStatus(clonedNimbleBuff, so_Status.CreateStatusProviderID(thisUnit, clonedNimbleBuff));
    }

    // this ignores auxiliaryInteract, so you can't accidentally immediately Wait
    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        thisUnit.ClaimReservation();
        return ExitSignal.ContinueTurn;
    }

    //
    private bool EnemyAt(Unit thisUnit, GridPosition gp) {
        if (!thisUnit.battleMap.IsInBounds(gp)) return false;
        Unit unitAt = thisUnit.unitMap.UnitAt(gp);
        return unitAt != null && unitAt.GetType() != thisUnit.GetType();
    }
}
