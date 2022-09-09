using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu (menuName = "TerrainEffects/GunkTerrainEffect")]
public class GunkTerrainEffect : TerrainEffect
{
    public so_Status gunkBuff;
    public so_Status gunkDebuff;

    public override string shortDisplayName { get; set; } = "-3 DR";

    public override void OnEnterTerrain(Unit targetUnit) {
        if (targetUnit.HasTagMatch("Gunk Enthusiast")) {
            targetUnit.statusSystem.AddStatus(gunkBuff, so_Status.CreateStatusProviderID(targetUnit, gunkBuff));
        } else {
            targetUnit.statusSystem.AddStatus(gunkDebuff, so_Status.CreateStatusProviderID(targetUnit, gunkDebuff));
        }
    }

    public override void OnExitTerrain(Unit targetUnit) {
        so_Status appliedStatus = (targetUnit.HasTagMatch("Gunk Enthusiast")) ? gunkBuff : gunkDebuff;
        targetUnit.statusSystem.RemoveStatus(so_Status.CreateStatusProviderID(targetUnit, appliedStatus));
    }
}