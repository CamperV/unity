using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Charge : Perk
{
    public override void OnAcquire() {
        boundUnit.OnMove += GainDamageBuffPerMove;

        //
        displayName = "Charge";
        boundUnit.buffManager.movementBuffProviders.Add("Charge");
    }

    public override void OnRemoval() {
        boundUnit.OnMove -= GainDamageBuffPerMove;
        boundUnit.buffManager.movementBuffProviders.Remove("Charge");
    }

    // adds a damage buff per square moved this turn
    private void GainDamageBuffPerMove(Path<GridPosition> pathTaken) {
        for (int i = 0; i < pathTaken.Count-1; i++) {
            boundUnit.buffManager.AddDamageBuff("Charge", 1, 1);
        }
    }
}