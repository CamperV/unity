using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class TacticsEntityBase : MovingObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	protected BoxCollider2D boxCollider2D;

	// constants for fade time, etc
	protected readonly float timeToDie = 1.0f;

	private bool _ghosted = false;
	public bool ghosted {
		get => _ghosted;
		set {
			_ghosted = value;
			SetTransparency((_ghosted) ? 0.5f : 1.0f);
		}
	}

	public float spriteHeight {
		get => spriteRenderer.bounds.size.y;
	}
	
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

	public override bool GridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir, GameManager.inst.tacticsManager.GetActiveGrid());
	}

	public virtual IEnumerator FadeDownToInactive(float fixedTime) {
		float timeRatio = 0.0f;
		Color ogColor = spriteRenderer.color;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			spriteRenderer.color = new Color(ogColor.r, ogColor.g, ogColor.b, (1.0f - timeRatio));
			yield return null;
		}
		gameObject.SetActive(false);
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
