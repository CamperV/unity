using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(EnemyUnit))]
public abstract class EnemyBrain : MonoBehaviour
{
    public abstract PlayerUnit GetOptimalTarget(List<PlayerUnit> targets);
    public abstract Vector3Int GetOptimalPositionToAttackTarget(Vector3Int targetPosition);

    // accessible to children

    // unit that this class is attached to
    protected EnemyUnit attachedUnit { get => GetComponent<EnemyUnit>(); }

	protected PlayerUnit GetClosestTarget(List<PlayerUnit> targets) {
		var minDist = targets.Min(it => it.gridPosition.ManhattanDistance(attachedUnit.gridPosition));
		return targets.First(it => it.gridPosition.ManhattanDistance(attachedUnit.gridPosition) == minDist);
	}
}



