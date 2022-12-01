using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/SeeingRedMut")]
public class SeeingRedMut : Mutation
{
    // apply a marked status to any unit that hits you
    public so_Status markStatus;
    public float damageMultiplier;

    public override void OnAcquire(Unit thisUnit) {
        thisUnit.OnHurtByTarget += ApplyMarkToAttacker;
        thisUnit.OnAttack += BonusDamageAgainstMark;
    }

    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnHurtByTarget -= ApplyMarkToAttacker;
        thisUnit.OnAttack -= BonusDamageAgainstMark;
    }

    private void ApplyMarkToAttacker(Unit thisUnit, Unit target) {
        string statusProviderID = so_Status.CreateStatusProviderID(thisUnit, markStatus);

        // first, remove all other instances from enemies, but only with the same provider (thisUnit)
        foreach (Unit enemy in thisUnit.Enemies()) {
            enemy.statusSystem.RemoveStatus(statusProviderID);
        }
        target.statusSystem.AddStatus(markStatus, statusProviderID);
    }

    private void BonusDamageAgainstMark(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        if (target.statusSystem.HasStatus(so_Status.CreateStatusProviderID(thisUnit, markStatus))) {
            mutAtt.AddBonusDamageMultiplier(damageMultiplier);
            mutAtt.AddMutator(this);
        }
    }
}