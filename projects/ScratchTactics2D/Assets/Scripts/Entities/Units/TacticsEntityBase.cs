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
	private bool _ghosted = false;
	public bool ghosted {
		get => _ghosted;
		set {
			_ghosted = value;
			SetTransparency((_ghosted) ? 0.5f : 1.0f);
		}
	}

	public float spriteHeight { get => spriteRenderer.bounds.size.y; }
	public virtual float zHeight { get => 1; }

	public static T Spawn<T>(T prefab, Vector3Int tilePos, TacticsGrid grid) where T : TacticsEntityBase {
		T entity = Instantiate(prefab, Vector3.zero, Quaternion.identity) as T;
			
		entity.gridPosition = tilePos;
		entity.UpdateRealPosition(grid.Grid2RealPos(entity.gridPosition));
		grid.UpdateOccupantAt(entity.gridPosition, entity);
		return entity;
	}
	
	protected virtual void Awake() {
		animator = GetComponent<Animator>();
		
		// set sprite properties
		//spriteRenderer.sortingLayerName = "Tactics Entities";
		spriteRenderer.sortingLayerName = "Tactics";
		spriteRenderer.sortingOrder = 0;

		// modify bounding box to match sprite
		boxCollider2D = GetComponent<BoxCollider2D>();
		Bounds spriteBounds = spriteRenderer.sprite.bounds;
		boxCollider2D.offset = spriteBounds.center;
	}

	public Sprite GetSprite() {
		return spriteRenderer.sprite;
	}

	public override void UpdateRealPosition(Vector3 pos) {
		transform.position = pos + new Vector3(0, 0, zHeight);
	}

	public bool ColliderContains(Vector3 v) {
		return boxCollider2D.bounds.Contains((Vector2)v);
	}

	public void RefreshColor() {
		spriteRenderer.color = Color.white;
	}
		
	private void SetTransparency(float val) {
		spriteRenderer.color = new Color(spriteRenderer.color.r,
										 spriteRenderer.color.g,
										 spriteRenderer.color.b,
										 val);
	}
}
