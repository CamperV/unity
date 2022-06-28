using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// these kinds of statuses exist for a certain amount of time, and then expire
[CreateAssetMenu(menuName = "Statuses/CountdownStatus")]
public class so_CountdownStatus : so_Status
{
	// assign in inspector
    public int value;

    public override void OnAcquire(Unit thisUnit){}
    public override void OnExpire(Unit thisUnit){}
}