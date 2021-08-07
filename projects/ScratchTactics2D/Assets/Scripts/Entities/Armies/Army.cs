using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public abstract class Army : MovingGridObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;

	// army stats/state
	public virtual float moveSpeed { get => 1.0f; }

	// constants
	protected readonly float timeToDie = 1.0f;
	
	public UnitController unitControllerPrefab;
	public Dictionary<string, Unit> unitPrefabs = new Dictionary<string, Unit>();
	public virtual List<string> defaultUnitTags { get; }

	// this is where the "real" units are stored
	private Dictionary<Guid, UnitState> barracks = new Dictionary<Guid, UnitState>();
	public int numUnits { get => barracks.Count; }
	
	// child classes must specify which grid to travel on
	public abstract bool GridMove(int xdir, int ydir);

	protected virtual void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		
		// generate your units here (name, tags, etc)
		PopulateBarracksFromTags(defaultUnitTags);
	}

	public static T Spawn<T>(T prefab, Vector3Int pos) where T : Army {
		T army = Instantiate(prefab, Vector3.zero, Quaternion.identity) as T;
		
		army.gridPosition = pos;
		army.UpdateRealPosition(GameManager.inst.overworld.Grid2RealPos(pos));
		GameManager.inst.overworld.UpdateOccupantAt(pos, army);
		return army;
	}

	public override void UpdateRealPosition(Vector3 pos) {
		transform.position = new Vector3(pos.x, pos.y, Constants.zSortingConstant);
	}

	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		UpdateRealPosition(GameManager.inst.overworld.Grid2RealPos(gridPosition));
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

	public void PopulateBarracksFromTags(List<string> unitTags) {
		foreach (string tag in unitTags) {
			PropertyInfo defaultProp = Type.GetType(tag).GetProperty("defaultState");
			UnitState defaultState = (UnitState)defaultProp.GetValue(null, null);
			//
			defaultState.ID = Guid.NewGuid();
			defaultState.unitName = $"{tag} Jeremy {Random.Range(0, 101)}";

			// now save the unit in our barracks
			EnlistUnit(defaultState);
		}
	}

	//
	//
	public void EnlistUnit(UnitState unitStats) {
		barracks[unitStats.ID] = unitStats;
	}
	public void EnlistUnit(Unit unit) {
		barracks[unit.unitStats.ID] = unit.unitStats;
	}

	public void DischargeUnit(UnitState unitStats) {
		barracks.Remove(unitStats.ID);
	}
	public void DischargeUnit(Unit unit) {
		barracks.Remove(unit.unitStats.ID);
	}

	public IEnumerable<UnitState> GetUnits() {
		foreach (var us in barracks.Values) {
			yield return us;
		}
	}

	public void Die() {
		// fade down
		// when faded, remove gameObject
		Debug.Log($"{this} has died :(");
		GameManager.inst.overworld.UpdateOccupantAt(gridPosition, null);
		StartCoroutine(FadeDownToInactive(timeToDie));
	}

	public IEnumerator FadeDownToInactive(float fixedTime) {
		float timeRatio = 0.0f;
		Color ogColor = spriteRenderer.color;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			spriteRenderer.color = new Color(ogColor.r, ogColor.g, ogColor.b, (1.0f - timeRatio));
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
