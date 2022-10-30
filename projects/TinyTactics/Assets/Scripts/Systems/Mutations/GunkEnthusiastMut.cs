using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/GunkEnthusiastMut")]
public class GunkEnthusiastMut : Mutation
{
    // apply a marked status to any unit that hits you
    public TerrainTile gunkTerrain;

    public override void OnAcquire(Unit thisUnit) {
        thisUnit.OnDeath += SpreadGunk;
        thisUnit.tags.Add("Gunk Enthusiast");
    }

    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnDeath -= SpreadGunk;
        thisUnit.tags.Remove("Gunk Enthusiast");
    }

    private void SpreadGunk(Unit thisUnit) {
        if (thisUnit.battleMap.TerrainAt(thisUnit.gridPosition) != gunkTerrain) {
            thisUnit.battleMap.CreateTerrainAt(thisUnit.gridPosition, gunkTerrain);
        }

        foreach (GridPosition gp in thisUnit.gridPosition.Radiate(1)) {
            if (gp == thisUnit.gridPosition || !thisUnit.battleMap.IsInBounds(gp)) continue;

            TerrainTile terrain = thisUnit.battleMap.TerrainAt(gp);
            if (terrain != gunkTerrain && terrain.cost != -1) {
                thisUnit.battleMap.CreateTerrainAt(gp, gunkTerrain);
            }
        }
    }
}