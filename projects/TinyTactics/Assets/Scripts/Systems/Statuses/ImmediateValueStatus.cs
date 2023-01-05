using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ImmediateValueStatus")]
public class ImmediateValueStatus : so_Status, IValueStatus, IImmediateStatus
{
	public static ImmediateValueStatus CloneWithValue(ImmediateValueStatus fromStatus, int newValue) {
		ImmediateValueStatus ivs = Instantiate(fromStatus);
        //
		ivs.value = newValue;
		return ivs;
	}

	// assign in inspector
	public StatSystem.UpdatableStat targetStat;
    
	// IValueStatus
	[field: SerializeField] public int value { get; set; }

    // IImmediateStatus
	[field: SerializeField] public bool revertWithMovement { get; set; }

    public override void OnAcquire(Unit thisUnit) {
		base.OnAcquire(thisUnit);
		thisUnit.statSystem.ModifyStat(targetStat, value);
	}
    public override void OnExpire(Unit thisUnit){
		base.OnExpire(thisUnit);
		thisUnit.statSystem.ModifyStat(targetStat, -value);
	}
}