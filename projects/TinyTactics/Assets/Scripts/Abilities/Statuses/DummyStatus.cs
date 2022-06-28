using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//
// This status doesn't actually do anything, but can be used for coupling entities
// For example, the SeeingRedMut affects the attacker if the defender has a "Mark"
// This mark can be a dummy mark
//
[CreateAssetMenu(menuName = "Statuses/DummyStatus")]
public class DummyStatus : so_Status
{
    public override void OnAcquire(Unit thisUnit) {}
    public override void OnExpire(Unit thisUnit) {}
}