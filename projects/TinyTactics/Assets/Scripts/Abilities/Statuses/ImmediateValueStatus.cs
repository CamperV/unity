using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ImmediateValueStatus")]
public class ImmediateValueStatus : so_Status, IValueStatus, IImmediateStatus
{
	// assign in inspector
	public UnitStats.UpdateableStat targetStat;
    
	// IValueStatus
	[field: SerializeField] public int value { get; set; }

    // IImmediateStatus
	[field: SerializeField] public bool revertWithMovement { get; set; }

    public override void OnAcquire(Unit thisUnit) {
		thisUnit.unitStats.ModifyStat(targetStat, value);
	}
    public override void OnExpire(Unit thisUnit){
		thisUnit.unitStats.ModifyStat(targetStat, -value);
	}
}