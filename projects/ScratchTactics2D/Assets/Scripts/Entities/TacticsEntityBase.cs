using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public class TacticsEntityBase : MovingObject
{
	// constants for fade time, etc
	protected readonly float timeToDie = 1.0f;

	protected Animator animator;

	protected int _animationStack;
	protected int animationStack {
		get => _animationStack;
		set {
			Debug.Assert(value > -1);
			_animationStack = value;
		}
	}

	protected BoxCollider2D boxCollider2D;
	private bool _ghosted = false;
	public bool ghosted {
		get => _ghosted;
		set {
			_ghosted = value;
			SetTransparency((_ghosted) ? 0.5f : 1.0f);
		}
	}

	protected SpriteRenderer spriteRenderer;
	public float spriteHeight { get => spriteRenderer.bounds.size.y; }
	
	// this is higher up, because we should be able to have Entities which are not Units
	public static TacticsEntityBase Spawn(TacticsEntityBase prefab, Vector3Int tilePos, TacticsGrid grid) {
		TacticsEntityBase entity = (TacticsEntityBase)Instantiate(prefab, grid.Grid2RealPos(tilePos), Quaternion.identity);
			
		entity.gridPosition = tilePos;
		grid.UpdateOccupantAt(entity.gridPosition, entity);
		return entity;
	}
	
	protected virtual void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		
		// set sprite properties
		spriteRenderer.sortingLayerName = "Tactics Entities";
		spriteRenderer.sortingOrder = 0;

		// modify bounding box to match sprite
		boxCollider2D = GetComponent<BoxCollider2D>();
		Bounds spriteBounds = spriteRenderer.sprite.bounds;
		boxCollider2D.offset = spriteBounds.center;
	}

	public Sprite GetSprite() {
		return spriteRenderer.sprite;
	}

	public override bool GridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir, GameManager.inst.tacticsManager.GetActiveGrid());
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

	//
	// Animation Coroutines
	//
	public bool IsAnimating() {
		return animationStack > 0;
	}

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (animationStack > 0) {
			yield return null;
		}
		VoidAction();
	}

	public virtual IEnumerator FadeDown(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}
	
	public IEnumerator FlashColor(Color color) {
		animationStack++;
		//

		var ogColor = spriteRenderer.color;

		float fixedTime = 1.0f;
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			var colorDiff = ogColor - ((1.0f - timeRatio) * (ogColor - color));
			spriteRenderer.color = colorDiff.WithAlpha(1.0f);

			yield return null;
		}
		spriteRenderer.color = ogColor;

		//
		animationStack--;
	}

	// not relative to time: shake only 3 times, wait a static amt of time
	public IEnumerator Shake(float radius) {
		animationStack++;
		//

		// I would love to do this in a one-liner...
		// (Select, etc) but due to Unity's choices, you cant' ToList a transform for its enumerated children
		// this should preserve order...?
		var ogPosition = transform.position;
		var childOgPositions = new List<Vector3>();
		foreach (Transform child in transform) childOgPositions.Add(child.position);
		int index;

		for (int i=0; i<3; i++) {
			Vector3 offset = (Vector3)Random.insideUnitCircle*radius;
			transform.position = ogPosition + offset;

			// reverse offset all children, so only the main Unit shakes
			index = 0;
			foreach (Transform child in transform) {
				child.position = childOgPositions[index] - offset;
				index++;
			}
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		transform.position = ogPosition;
		index = 0;
		foreach (Transform child in transform) {
			child.position = childOgPositions[index];
			index++;
		}

		//
		animationStack--;
	}
}
