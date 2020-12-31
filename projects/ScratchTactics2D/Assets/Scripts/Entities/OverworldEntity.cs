using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class OverworldEntity : MovingObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	
	public Controller unitControllerPrefab;
	//
	public Dictionary<string, Unit> unitPrefabs = new Dictionary<string, Unit>();
	public virtual List<string> defaultUnitTags { get; }

	// this is where the "real" units are stored
	public Dictionary<Guid, UnitStats> barracks = new Dictionary<Guid, UnitStats>();
	
	protected virtual void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		
		// generate your units here (name, tags, etc)
		GenerateUnitStats();
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

	public void GenerateUnitStats() {
		foreach (string tag in defaultUnitTags) {
			PropertyInfo defaultProp = Type.GetType(tag).GetProperty("defaultStats");
			UnitStats defaultStats = (UnitStats)defaultProp.GetValue(null, null);
			//
			defaultStats.ID = Guid.NewGuid();
			defaultStats.unitName = $"{tag} Jeremy {Random.Range(0, 101)}";

			// now save the unit in our barracks
			barracks[defaultStats.ID] = defaultStats;
			Debug.Log($"{this} generated {barracks[defaultStats.ID]}");
		}
	}

	//
	//
	public void RemoveUnit(Guid id) {
		barracks.Remove(id);
	}

	public void Die() {
		// fade down
		// when faded, remove gameObject
		Debug.Log($"{this} has died :(");
		GameManager.inst.worldGrid.UpdateOccupantAt(gridPosition, null);
		StartCoroutine(FadeDownToInactive(0.01f));
	}

	public IEnumerator FadeDownToInactive(float fadeRate) {
		float c = 1.0f;
		while (c > 0.0f) {
			spriteRenderer.color = new Color(spriteRenderer.color.r,
											 spriteRenderer.color.g,
											 spriteRenderer.color.b,
											 c);
			c -= fadeRate;
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
