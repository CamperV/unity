using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public class TacticsEntityBase : MovingGridObject
{
	// constants for fade time, etc
	protected readonly float timeToDie = 1.0f;
	protected Animator animator;

	protected BoxCollider2D boxCollider2D;

	public bool mouseOver { get; private set; }
	public bool clickable { get; set; }

	public float spriteHeight { get => spriteRenderer.bounds.size.y; }

	public static T Spawn<T>(T prefab, Vector3Int tilePos, TacticsGrid grid) where T : TacticsEntityBase {
		T entity = Instantiate(prefab, Vector3.zero, Quaternion.identity) as T;
		
		entity.UpdateGridPosition(tilePos, grid);
		grid.UpdateOccupantAt(entity.gridPosition, entity);
		return entity;
	}
	
	protected virtual void Awake() {
		animator = GetComponent<Animator>();

		// modify bounding box to match sprite (DEPRECATED, THIS IS DONE VIA PREFAB FLOW NOW)
		boxCollider2D = GetComponent<BoxCollider2D>();
		mouseOver = false;
		clickable = true;
	}

	void OnMouseEnter() { mouseOver = true; }
	void OnMouseExit() { mouseOver = false; }

	public Sprite GetSprite() {
		return spriteRenderer.sprite;
	}

	public override void UpdateRealPosition(Vector3 pos) {
		// transform.position = pos + new Vector3(0, 0, 1f);
		transform.position = pos;
	}

	public void RefreshColor() {
		spriteRenderer.color = Color.white;
	}
		
	public void SetTransparency(float val) {
		spriteRenderer.color = new Color(spriteRenderer.color.r,
										 spriteRenderer.color.g,
										 spriteRenderer.color.b,
										 val);
	}
}
