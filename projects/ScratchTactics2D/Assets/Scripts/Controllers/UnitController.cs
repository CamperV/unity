using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class UnitController : Controller
{
	public const float postPhaseDelay = 1.0f;
		
	public override void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
		activeRegistry.ForEach(u => ((Unit)u).OnStartTurn());
	}

	public override void EndPhase(float timeOverride = phaseDelayTime) {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(timeOverride, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		}));
		activeRegistry.ForEach(u => ((Unit)u).RefreshColor());
	}

	// if any unit is active at all, don't end the phase
	public bool EndPhaseNow() {
		foreach (Unit unit in activeRegistry) {
			if (unit.turnActive) {
				return false;
			}
		}
		return true;
	}

	public List<MovingObject> GetRegisteredInBattle() {
		var controllers = GameManager.inst.tacticsManager.activeBattle.GetActiveControllers();
		return controllers.SelectMany(con => con.activeRegistry).ToList();
	}
	
	public List<MovingObject> GetOpposing() {
		var advPhase = myPhase.NextPhase();
		var advController = GameManager.inst.tacticsManager.activeBattle.GetControllerFromPhase(advPhase);
		return advController.activeRegistry;
	}

	public HashSet<Vector3Int> GetObstacles() {
		// the controller must dictate to the Unit/MovingObject what counts as obstacles for it
		var uPositions = GetOpposing().Select(it => it.gridPosition);
		return new HashSet<Vector3Int>(uPositions);
	}
}