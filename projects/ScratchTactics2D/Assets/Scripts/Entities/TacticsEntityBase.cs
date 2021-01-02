using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class TacticsEntityBase : MovingObject
{
	protected SpriteRenderer spriteRenderer;
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
	
	// this is higher up, because we should be able to have Entities which are not Units
	public static TacticsEntityBase Spawn(TacticsEntityBase prefab, Vector3Int tilePos, TacticsGrid grid) {
		TacticsEntityBase entity = (TacticsEntityBase)Instantiate(prefab, grid.Grid2RealPos(tilePos), Quaternion.identity);
			
		entity.gridPosition = tilePos;
		grid.UpdateOccupantAt(entity.gridPosition, entity);
		return entity;
	}
	
	protected void Awake() {
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

	public IEnumerator FadeDownToInactive(float fadeRate) {
		float c = 1.0f;
		while (c > 0.0f) {
			spriteRenderer.color = new Color(1, 1, 1, c);
			c -= fadeRate;
			yield return null;
		}
		gameObject.SetActive(false);
	}

	public bool ColliderContains(Vector3 v) {
		return boxCollider2D.bounds.Contains((Vector2)v);
	}
		
	private void SetTransparency(float val) {
		var currColor = spriteRenderer.color;
		currColor.a = val;
		spriteRenderer.color = currColor;
	}
}
