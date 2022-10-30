using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/PermanentEngagementStatus")]
public class PermanentEngagementStatus : EngagementStatus
{
	public static PermanentEngagementStatus CloneWithValue(PermanentEngagementStatus fromStatus, int newValue) {
		PermanentEngagementStatus ies = Instantiate(fromStatus);
        //
		ies.value = newValue;
		return ies;
	}
}