using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class UnitController : Controller
{	
	// this is seperate from PhasedObject.phaseDelayTime
	// and thus, is not accelerated by the GameManager listener
	public const float postPhaseDelayTime = 1.0f;
		
	public override void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
		activeRegistry.ForEach(u => ((Unit)u).OnStartTurn());
	}

	public override void EndPhase() {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(postPhaseDelayTime, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		}));
		activeRegistry.ForEach(u => (u as Unit).RefreshColor());
	}

	// if any unit is active at all, don't end the phase
	public bool EndPhaseNow() {
		foreach (Unit unit in activeRegistry) {
			if (unit.turnActive) return false;
		}
		return true;
	}
	
	public List<MovingGridObject> GetOpposing() {
		var advPhase = myPhase.NextPhase();
		var advController = GameManager.inst.tacticsManager.activeBattle.GetControllerFromPhase(advPhase);
		return advController.activeRegistry;
	}

	public virtual HashSet<Vector3Int> GetObstacles() {
		// the controller must dictate to the Unit/MovingGridObject what counts as obstacles for it
		var uPositions = GetOpposing().Select(it => it.gridPosition);
		return new HashSet<Vector3Int>(uPositions);
	}
}