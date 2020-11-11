using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsEntityBase : MovingObject
{
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	
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
	}

	public override bool GridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir, GameManager.inst.tacticsManager.GetActiveGrid());
	}
}
