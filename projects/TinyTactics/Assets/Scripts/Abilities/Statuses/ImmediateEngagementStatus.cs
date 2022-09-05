using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/ImmediateEngagementStatus")]
public class ImmediateEngagementStatus : EngagementStatus, IImmediateStatus
{
	public static ImmediateEngagementStatus CloneWithValue(ImmediateEngagementStatus fromStatus, int newValue) {
		ImmediateEngagementStatus ies = Instantiate(fromStatus);
        //
		ies.value = newValue;
		return ies;
	}

    // IImmediateStatus
	[field: SerializeField] public bool revertWithMovement { get; set; }
}