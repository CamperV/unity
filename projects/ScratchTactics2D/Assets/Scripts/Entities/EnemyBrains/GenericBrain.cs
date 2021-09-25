using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Extensions;

public class GenericBrain : EnemyBrain
{
    // General priorities:
    // 1) in attack range
    // 2) highest damage
    // 3) longest range
    // 4) best chance to hit
    // 5) best chance to crit
	public override PlayerUnit GetOptimalTarget(List<PlayerUnit> targets) {
        // all targets in the attack range
		List<PlayerUnit> attackTargets = targets.FindAll(it => attachedUnit.attackRange.field.ContainsKey(it.gridPosition));

        if (attackTargets.Any()) {
            return GetClosestTarget(attackTargets);
        } else {
            return GetClosestTarget(targets);
        }
    }

	public override Vector3Int GetOptimalPositionToAttackTarget(Vector3Int targetPosition) {
		bool SubjectCanStand(Vector3Int v) {
			return Battle.active.grid.IsInBounds(v) && (Battle.active.grid.VacantAt(v) || v == attachedUnit.gridPosition);
		}
		float DistToTarget(Vector3Int v) { return targetPosition.ManhattanDistance(v); }
		float DistToAttachedUnit(Vector3Int v) { return attachedUnit.gridPosition.ManhattanDistance(v); }
		// util

		// max allowable attack positions (max range/reach)
		// NOTE: need to Radiate again if the sequence is empty.
		var targetable = targetPosition.GridRadiate(Battle.active.grid, attachedUnit._RANGE).Where(it => SubjectCanStand(it));
		float maxDistWithin = targetable.Max(it => DistToTarget(it));
		var atMaxDist = targetable.Where(it => DistToTarget(it) == maxDistWithin);

		// the closest of those to the acting attachedUnit
		float minDistSubject = atMaxDist.Min(it => DistToAttachedUnit(it));
		var optimalPosition = atMaxDist.First(it => DistToAttachedUnit(it) == minDistSubject);
		return optimalPosition;
    }
}



