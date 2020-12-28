using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

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
		foreach (string tag in defaultUnitTags) {
			UnitStats stats = new UnitStats(tag);
			barracks[stats.id] = stats;
		}
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
