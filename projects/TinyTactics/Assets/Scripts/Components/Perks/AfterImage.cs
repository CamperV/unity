using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AfterImage : Perk
{
    public override void OnAcquire() {
        boundUnit.OnAvoid += RefreshMovement;
        //
        displayName = "Afterimage";
    }

    public override void OnRemoval() {
        boundUnit.OnAvoid -= RefreshMovement;
    }

    private void RefreshMovement() {
        boundUnit.moveAvailable = true;

        string unitTag = (boundUnit.GetType() == typeof(PlayerUnit)) ? "PLAYER_UNIT" : "ENEMY_UNIT";
        UIManager.inst.combatLog.AddEntry($"BLUE@{displayName} granted additional movement {unitTag}@{boundUnit.name}.");
    }
}