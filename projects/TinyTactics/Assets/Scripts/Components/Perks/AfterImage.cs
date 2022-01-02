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

        UIManager.inst.combatLog.AddEntry($"BLUE@[{displayName}] granted additional movement {boundUnit.logTag}@[{boundUnit.displayName}].");
    }
}