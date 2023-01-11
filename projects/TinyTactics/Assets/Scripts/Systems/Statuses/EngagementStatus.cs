using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EngagementStatus : so_Status, IValueStatus
{
    public enum ModifyStat {
        Damage,
        PoiseDamage,
        CriticalRate,
        DamageReduction,
        CriticalAvoid
    };
    public ModifyStat modifyStat;

    // IValueStatus
	[field: SerializeField] public int value { get; set; }

    public override void OnAcquire(Unit thisUnit) {
        base.OnAcquire(thisUnit);
        thisUnit.OnAttackGeneration += ModifyAttack;
	}

    public override void OnExpire(Unit thisUnit) {
        base.OnExpire(thisUnit);
        thisUnit.OnAttackGeneration -= ModifyAttack;
    }

    private void ModifyAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        switch (modifyStat) {
            case ModifyStat.Damage:
                mutAtt.damage.Add(value);
                break;

            case ModifyStat.PoiseDamage:
                mutAtt.poiseDamage.Add(value);
                break;

            case ModifyStat.CriticalRate:
                mutAtt.critRate += value;
                break;

            case ModifyStat.DamageReduction:
                mutAtt.damageReduction += value;
                break;

            case ModifyStat.CriticalAvoid:
                mutAtt.critAvoidRate += value;
                break;

            default:
                Debug.LogError($"Invalid configuration. ImmediateEngagementStatus cannot have modify type {modifyStat}");
                break;
        }
    }
}