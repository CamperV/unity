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
        thisUnit.OnHurtBy += ApplyMarkToAttacker;
        thisUnit.OnAttack += BonusDamageAgainstMark;
    }

    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnHurtBy -= ApplyMarkToAttacker;
        thisUnit.OnAttack -= BonusDamageAgainstMark;
    }

    private void ApplyMarkToAttacker(Unit thisUnit, Unit target) {
        target.statusSystem.AddStatus(markStatus);
    }

    private void BonusDamageAgainstMark(ref MutableAttack mutAtt, Unit target) {
        if (target.statusSystem.HasStatus(markStatus)) {
            mutAtt.minDamage = (int)(mutAtt.minDamage*damageMultiplier);
            mutAtt.maxDamage = (int)(mutAtt.maxDamage*damageMultiplier);

            mutAtt.AddMutator(this);
        }
    }
}