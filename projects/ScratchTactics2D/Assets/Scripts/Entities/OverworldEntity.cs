using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class OverworldEntity : MovingObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	
	public Dictionary<string, Unit> unitPrefabs = new Dictionary<string, Unit>();
	//
	public virtual List<string> defaultUnitTags { get; }
	
	protected void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}
	
	public List<Unit> LoadUnitsByTag(List<string> unitTags) {
		List<Unit> retVal = new List<Unit>();
		// we determine initial spawn location by simply assigning them zip-style
		for (int i=0; i < unitTags.Count; i++) {
			Unit unitPrefab;
			if (unitPrefabs.ContainsKey(unitTags[i])) {
				unitPrefab = unitPrefabs[unitTags[i]];
			} else {
				unitPrefab = Resources.Load<Unit>(unitTags[i]);
				unitPrefabs.Add(unitTags[i], unitPrefab);
			}
			retVal.Add(unitPrefab);
		}
		return retVal;
	}
}
