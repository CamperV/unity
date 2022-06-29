using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ImmediateValueStatus")]
public class ImmediateValueStatus : so_Status, IValueStatus
{
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