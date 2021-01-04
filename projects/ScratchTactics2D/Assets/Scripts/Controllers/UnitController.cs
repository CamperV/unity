using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class UnitController : Controller
{
	public void RefreshAllUnits() {
		foreach (Unit unit in activeRegistry) {
			unit.RefreshOptions();
		}
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