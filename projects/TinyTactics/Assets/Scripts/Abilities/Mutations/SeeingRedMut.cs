using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu (menuName = "Mutations/SeeingRedMut")]
public class SeeingRedMut : Mutation
{
    public int damageBonus;

    public override void OnAcquire(Unit thisUnit) {
        thisUnit.OnMove += GainDamageBuffPerMove;
        thisUnit.statusManager.movementBuffProviders.Add(name);
    }

    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnMove -= GainDamageBuffPerMove;
        thisUnit.statusManager.movementBuffProviders.Remove(name);
    }

    // adds a damage buff per space moved this turn
    private void GainDamageBuffPerMove(Unit thisUnit, Path<GridPosition> pathTaken) {
        for (int i = 0; i < pathTaken.Count-1; i++) {
            thisUnit.statusManager.AddValuedStatus<OneTimeDamageBuff>(name, damageBonus);
        }
    }
}