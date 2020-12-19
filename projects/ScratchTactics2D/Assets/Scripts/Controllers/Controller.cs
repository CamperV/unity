using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Controller : PhasedObject
{
	public List<MovingObject> registry;
	public List<MovingObject> activeRegistry {
		get {
			return registry.Where(it => it.IsActive()).ToList();
		}
	}

	protected void Awake() {		
		registry = new List<MovingObject>();
    }
	
	public virtual void Register(MovingObject subject) {
		registry.Add(subject);

		if (subject.transform.parent != null) {
			throw new System.InvalidOperationException($"{subject} is already registered to Controller {subject.transform.parent}, unassign and try again");
		}
		subject.transform.parent = transform;
	}

	public void RefreshAllUnits() {
		foreach (Unit unit in activeRegistry) {
			unit.RefreshOptions();
		}
	}

	public HashSet<Vector3Int> GetObstacles() {
		// the controller must dictate to the Unit/MovingObject what counts as obstacles for it
		var adversaryPhase = myPhase.NextPhase();
		var adversaryController = GameManager.inst.tacticsManager.activeBattle.GetControllerFromPhase(adversaryPhase);
		var uPositions = from u in adversaryController.activeRegistry select u.gridPosition;
		return new HashSet<Vector3Int>(uPositions);
	}
}