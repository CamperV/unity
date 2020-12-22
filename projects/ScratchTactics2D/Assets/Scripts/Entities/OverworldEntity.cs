using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class OverworldEntity : MovingObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	
	public Controller unitControllerPrefab;
	[HideInInspector] public Controller unitController;
	//
	public Dictionary<string, Unit> unitPrefabs = new Dictionary<string, Unit>();
	//
	public virtual List<string> defaultUnitTags { get; }
	
	protected virtual void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	protected void Update() {
		//spriteRenderer.sortingOrder = -1*Mathf.RoundToInt(transform.position.y);
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
