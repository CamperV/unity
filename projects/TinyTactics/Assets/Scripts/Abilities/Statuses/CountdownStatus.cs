using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// these kinds of statuses exist for a certain amount of time, and then expire
[CreateAssetMenu(menuName = "Statuses/CountdownStatus")]
public class CountdownStatus : so_Status, IValueStatus, IExpireStatus
{
    // IValueStatus
	[field: SerializeField] public int value { get; set; }

	// IExpireStatus
	[field: SerializeField] public int expireTimer { get; set; }

	// assign in inspector
	public UnitStats.UpdateableStat targetStat;

    public override void OnAcquire(Unit thisUnit) {
		base.OnAcquire(thisUnit);
		thisUnit.unitStats.ModifyStat(targetStat, value);
	}
	
    public override void OnExpire(Unit thisUnit){
		base.OnExpire(thisUnit);
		thisUnit.unitStats.ModifyStat(targetStat, -value);
	}
}