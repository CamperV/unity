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
        thisUnit.unitPathfinder.moveThroughEnemiesOverride = true;
        thisUnit.unitPathfinder.loweredTerrainCostOverride = true;
        
        base.Activate(thisUnit);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        base.Deactivate(thisUnit);
        
        thisUnit.unitPathfinder.moveThroughEnemiesOverride = false;
        thisUnit.unitPathfinder.loweredTerrainCostOverride = false;
    }

    protected override void OnMoveEffects(Unit thisUnit, Path<GridPosition> pathTaken) {
        int passedEnemies = 0;
        foreach (GridPosition gp in pathTaken.Unwind()) {
            if (EnemyAt(thisUnit, gp)) passedEnemies++;
        }
    
        if (passedEnemies > 0) {
            ProcFor(thisUnit);  // let the world know you've proc'd

            ImmediateValueStatus clonedScurryBuff = ImmediateValueStatus.CloneWithValue(scurryBuff, passedEnemies);
            thisUnit.statusSystem.AddStatus(clonedScurryBuff, so_Status.CreateStatusProviderID(thisUnit, clonedScurryBuff));
        }
    }

    //
    private bool EnemyAt(Unit thisUnit, GridPosition gp) {
        if (!thisUnit.battleMap.IsInBounds(gp)) return false;
        Unit unitAt = thisUnit.unitMap.UnitAt(gp);
        return unitAt != null && unitAt.GetType() != thisUnit.GetType();
    }
}
