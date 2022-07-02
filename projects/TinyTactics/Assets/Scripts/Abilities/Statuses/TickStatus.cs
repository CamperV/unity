using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/TickStatus")]
public class TickStatus : so_Status, IValueStatus, IExpireStatus
{
	// assign in inspector
	public UnitStats.UpdateableStat targetStat;
    
	// IValueStatus
	[field: SerializeField] public int value { get; set; }
	
	// IExpireStatus
	public int expireTimer {
		get => value;
		set {}
	}

    public override void OnAcquire(Unit thisUnit){
		base.OnAcquire(thisUnit);
		Apply(thisUnit, value);
	}

	public void Apply(Unit thisUnit, int _value) {
		thisUnit.unitStats.ModifyStat(targetStat, _value);
	}
}