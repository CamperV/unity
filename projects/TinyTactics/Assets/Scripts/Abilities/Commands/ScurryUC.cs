using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/ScurryUC")]
public class ScurryUC : MoveUC
{
    public ImmediateValueStatus scurryBuff;

    public override void Activate(PlayerUnit thisUnit) {
        // ScurryUC
        thisUnit.unitPathfinder.moveThroughEnemiesOverride = true;
        // ScurryUC
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
    
        ImmediateValueStatus clonedScurryBuff = ImmediateValueStatus.CloneWithValue(scurryBuff, passedEnemies);
        thisUnit.statusSystem.AddStatus(clonedScurryBuff, so_Status.CreateStatusProviderID(thisUnit, clonedScurryBuff));
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
