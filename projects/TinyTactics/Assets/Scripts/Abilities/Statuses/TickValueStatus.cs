using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/TickValueStatus")]
public class TickValueStatus : so_Status, IValueStatus
{
	// public static TickValueStatus Create(UnitStats.UpdateableStat targetStat, int value) {
	// 	TickValueStatus vs = ScriptableObject.CreateInstance<TickValueStatus>() as TickValueStatus;
	// 	vs.targetStat = targetStat;
	// 	vs.value = value;
	// 	return vs;
	// }

	// assign in inspector
	public UnitStats.UpdateableStat targetStat;
    
	// IValueStatus
	[field: SerializeField] public int value { get; set; }

    public override void OnAcquire(Unit thisUnit){}
    public override void OnExpire(Unit thisUnit){}

	public void Apply(Unit thisUnit, int _value) {
		thisUnit.unitStats.ModifyStat(targetStat, _value);
	}
}