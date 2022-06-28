using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ValueStatus")]
public class so_ValueStatus : so_Status
{
	public static so_ValueStatus Create(UnitStats.UpdateableStat targetStat, int value) {
		so_ValueStatus vs = ScriptableObject.CreateInstance<so_ValueStatus>() as so_ValueStatus;
		vs.targetStat = targetStat;
		vs.value = value;
		return vs;
	}

	// assign in inspector
	public UnitStats.UpdateableStat targetStat;
    public int value;

	// this is just to more easily switch b/w functions for expiration modes across objects
	public enum ExpirationType {
		Tick,
		Immediate
	}
	public ExpirationType expirationType;

    public override void OnAcquire(Unit thisUnit){}
    public override void OnExpire(Unit thisUnit){}

	public void Apply(Unit thisUnit, int _value) {
		thisUnit.unitStats.ModifyStat(targetStat, _value);
	}
}