using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EngagementStatus : so_Status, IValueStatus, IMutatorComponent
{
    public enum RegisterMode {
        OnAttack,
        OnDefend
    };
    public RegisterMode registerTo;

    public enum ModifyStat {
        Damage,
        CriticalRate,
        DamageReduction,
        CriticalAvoid
    };
    public ModifyStat modifyStat;

    // IValueStatus
	[field: SerializeField] public int value { get; set; }

    // IMutatorComponent
	public string displayName {
		get => $"{name} (+{value})";
		set {}
	}

    public override void OnAcquire(Unit thisUnit) {
        base.OnAcquire(thisUnit);
        
		switch (registerTo) {
            case RegisterMode.OnAttack:
                thisUnit.OnAttack += ModifyAttack;
                break;

            case RegisterMode.OnDefend:
                thisUnit.OnDefend += ModifyDefend;
                break;
        }
	}

    public override void OnExpire(Unit thisUnit) {
        base.OnExpire(thisUnit);

        switch (registerTo) {
            case RegisterMode.OnAttack:
                thisUnit.OnAttack -= ModifyAttack;
                break;

            case RegisterMode.OnDefend:
                thisUnit.OnDefend -= ModifyDefend;
                break;
        }
    }

    private void ModifyAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        switch (modifyStat) {
            case ModifyStat.Damage:
                mutAtt.AddBonusDamage(value);
                break;

            case ModifyStat.CriticalRate:
                mutAtt.critRate += value;
                break;

            default:
                Debug.LogError($"Invalid configuration. ImmediateEngagementStatus cannot have modify type {modifyStat} with register mode {registerTo}");
                break;
        }
    }

    private void ModifyDefend(Unit thisUnit, ref MutableDefense mutDef, Unit attacker) {
        switch (modifyStat) {
            case ModifyStat.DamageReduction:
                mutDef.damageReduction += value;
                break;

            case ModifyStat.CriticalAvoid:
                mutDef.critAvoidRate += value;
                break;
            
            default:
                Debug.LogError($"Invalid configuration. ImmediateEngagementStatus cannot have modify type {modifyStat} with register mode {registerTo}");
                break;
        }
    }
}