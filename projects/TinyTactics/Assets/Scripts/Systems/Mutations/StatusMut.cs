using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/StatusMut")]
public class StatusMut : Mutation
{
    public so_Status status;

    public override void OnAcquire(Unit thisUnit) {
        thisUnit.statusSystem.AddStatus(status, so_Status.CreateStatusProviderID(thisUnit, status));
	}

    public override void OnRemove(Unit thisUnit) {
        thisUnit.statusSystem.RemoveStatus(so_Status.CreateStatusProviderID(thisUnit, status));
	}
}