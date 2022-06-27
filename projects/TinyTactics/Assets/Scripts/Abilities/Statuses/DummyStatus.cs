using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//
// This mark doesn't actually do anything, other than give an entity 
// with SeeingRedMut a place to direct their rage
//
[CreateAssetMenu(menuName = "Statuses/DummyStatus")]
public class DummyStatus : so_Status
{
    public override void OnAcquire(Unit thisUnit) {}
    public override void OnRemove(Unit thisUnit) {}
}