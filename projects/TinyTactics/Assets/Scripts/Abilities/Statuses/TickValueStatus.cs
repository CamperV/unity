using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/TickValueStatus")]
public class TickValueStatus : so_Status, IValueStatus, IExpireStatus
{
	// assign in inspector
	public UnitStats.UpdateableStat targetStat;
    
	// IValueStatus
	[field: SerializeField] public int value { get; set; }

    public override void OnAcquire(Unit thisUnit){
		Apply(thisUnit, value);
	}
    public override void OnExpire(Unit thisUnit){}

	public void Apply(Unit thisUnit, int _value) {
		thisUnit.unitStats.ModifyStat(targetStat, _value);
	}
}