﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public abstract class Army : MovingGridObject
{
	public abstract string armyTag { get; }
	protected Animator animator;

	// army stats/state
	public virtual float moveSpeed { get => 1.0f; }

	// constants
	protected readonly float timeToDie = 1.0f;
	
	public UnitController unitControllerPrefab;
	public Dictionary<string, Unit> unitPrefabs = new Dictionary<string, Unit>();

	// this is where the "real" units are stored
	private Dictionary<Guid, UnitState> barracks = new Dictionary<Guid, UnitState>();
	public int numUnits { get => barracks.Count; }
	
	// child classes must specify which grid to travel on
	public abstract bool GridMove(int xdir, int ydir);

	public static T Spawn<T>(T prefab, Vector3Int pos) where T : Army {
		T army = Instantiate(prefab, Vector3.zero, Quaternion.identity) as T;
		
		GameManager.inst.overworld.UpdateOccupantAt(pos, army);
		army.UpdateGridPosition(pos, GameManager.inst.overworld);
		return army;
	}

	public override void UpdateRealPosition(Vector3 pos) {
		transform.position = new Vector3(pos.x, pos.y, Constants.zSortingConstant);
	}

	public Unit LoadUnitByTag(string tag) {
		Unit unitPrefab;
		if (unitPrefabs.ContainsKey(tag)) {
			unitPrefab = unitPrefabs[tag];
		} else {
			unitPrefab = Resources.Load<Unit>(tag);
			unitPrefabs.Add(tag, unitPrefab);
		}
		return unitPrefab;
	}
		
	public List<Unit> LoadUnitsByTag(List<string> unitTags) {
		return unitTags.Select(it => LoadUnitByTag(it)).ToList();
	}
	public abstract void PopulateBarracksFromTags(List<string> unitTags);

	public void EnlistUnit(UnitState unitState) {
		barracks[unitState.ID] = unitState;
	}
	public void EnlistUnit(Unit unit) {
		barracks[unit.unitState.ID] = unit.unitState;
	}

	public void DischargeUnit(UnitState unitState) {
		barracks.Remove(unitState.ID);
	}
	public void DischargeUnit(Unit unit) {
		barracks.Remove(unit.unitState.ID);
	}

	public IEnumerable<UnitState> GetUnitStates() {
		foreach (var us in barracks.Values) {
			yield return us;
		}
	}

	public void Die() {
		// fade down
		// when faded, remove gameObject
		Debug.Log($"{this} has died :(");
		GameManager.inst.overworld.UpdateOccupantAt(gridPosition, null);
		StartCoroutine( spriteAnimator.FadeDownThen(timeToDie, () => {
			gameObject.SetActive(false);
		}) );
	}
}
