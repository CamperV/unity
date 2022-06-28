using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Extensions;

public class BrainPod : MonoBehaviour
{	
	// set in inspector
	public List<EnemyUnit> podmates;
	public int Initiative => (podmates.Count > 0) ? podmates.Min(unit => unit.Initiative) : Int32.MaxValue;

	public HashSet<GridPosition> sharedMoveRangeDimensions;
	public HashSet<GridPosition> sharedTargetRangeDimensions;

	void Awake() {
		podmates = new List<EnemyUnit>();

		foreach (EnemyUnit unit in GetComponentsInChildren<EnemyUnit>()) {
			unit.assignedPod = this;
			podmates.Add(unit);
		}

		sharedMoveRangeDimensions = new HashSet<GridPosition>();
		sharedTargetRangeDimensions = new HashSet<GridPosition>();
	}

	public void UpdateSharedDetectionRange() {
		sharedMoveRangeDimensions.Clear();
		sharedTargetRangeDimensions.Clear();

		foreach (EnemyUnit unit in podmates) {
			foreach (GridPosition gp in unit.moveRange.field.Keys) {
				sharedMoveRangeDimensions.Add(gp);
			}
			foreach (GridPosition gp in unit.attackRange.field.Keys) {
				sharedTargetRangeDimensions.Add(gp);
			}
		}
	}
}



