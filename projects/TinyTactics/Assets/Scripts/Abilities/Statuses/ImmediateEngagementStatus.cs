using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ImmediateEngagementStatus")]
public class ImmediateEngagementStatus : so_Status, IValueStatus, IImmediateStatus, IMutatorComponent
{
	public static ImmediateEngagementStatus CloneWithValue(ImmediateEngagementStatus fromStatus, int newValue) {
		ImmediateEngagementStatus ies = Instantiate(fromStatus);
        //
		ies.value = newValue;
		return ies;
	}

    public enum RegisterMode {
        OnAttack,
        OnDefend
    };
    public RegisterMode registerTo;

    public enum ModifyStat {
        Damage,
        CriticalRate,
        AdvantageRate
    };
    public ModifyStat modifyStat;

    // IValueStatus
	[field: SerializeField] public int value { get; set; }

    // IImmediateStatus
	[field: SerializeField] public bool revertWithMovement { get; set; }

    // IMutatorComponent
	public string displayName {
		get => $"{name} (+{value})";
		set {}
	}

    public override void OnAcquire(Unit thisUnit) {
		switch (registerTo) {
            case RegisterMode.OnAttack:
                thisUnit.OnAttack += ModifyAttack;
                break;

            case RegisterMode.OnDefend:
                thisUnit.OnDefend += ModifyDefend;
                break;
        }
	}

    public override void OnExpire(Unit thisUnit){
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
                mutAtt.AddDamage(value);
                break;

            case ModifyStat.CriticalRate:
                mutAtt.critRate += value;
                break;

            case ModifyStat.AdvantageRate:
                mutAtt.dexterity += value;
                break;
        }

        mutAtt.AddMutator(this);
    }

    private void ModifyDefend(Unit thisUnit, ref MutableDefense mutDef, Unit attacker) {
        switch (modifyStat) {
            case ModifyStat.Damage:
                mutDef.damageReduction += value;
                break;

            case ModifyStat.CriticalRate:
                mutDef.critAvoidRate += value;
                break;

            case ModifyStat.AdvantageRate:
                mutDef.reflex += value;
                break;
        }

        mutDef.AddMutator(this);
    }
}