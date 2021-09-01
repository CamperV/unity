using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public abstract class UnitController : Controller
{	
	// this is seperate from PhasedObject.phaseDelayTime
	// and thus, is not accelerated by the GameManager listener
	public const float postPhaseDelayTime = 1.0f;

	// if any unit is active at all, don't end the phase
	public bool EndPhaseNow() {
		foreach (Unit unit in activeRegistry) {
			if (unit.turnActive) return false;
		}
		return true;
	}
	
	public abstract List<MovingGridObject> GetOpposing();
	public abstract HashSet<Vector3Int> GetObstacles();
}