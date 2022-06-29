using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// these kinds of statuses exist for a certain amount of time, and then expire
[CreateAssetMenu(menuName = "Statuses/CountdownStatus")]
public class CountdownStatus : so_Status, IValueStatus
{
	// assign in inspector
    // IValueStatus
	[field: SerializeField] public int value { get; set; }

    public override void OnAcquire(Unit thisUnit){}
    public override void OnExpire(Unit thisUnit){}
}