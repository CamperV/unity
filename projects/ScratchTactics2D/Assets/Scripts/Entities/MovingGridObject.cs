using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(SpriteAnimator))]
public abstract class MovingGridObject : MonoBehaviour, IMovable
{	
	// friendly components for helping
	protected SpriteRenderer spriteRenderer { get => GetComponent<SpriteRenderer>(); }
	public SpriteAnimator spriteAnimator { get => GetComponent<SpriteAnimator>(); }

	protected Vector3Int _gridPosition;
	[HideInInspector] public virtual Vector3Int gridPosition {
		get => _gridPosition;
		protected set {
			_gridPosition = value;			
		}
	}
	
	protected Coroutine crtMovement;

	// IMovable defs
	public virtual void UpdateRealPosition(Vector3 pos) {
		transform.position = pos;
	}
	
	public void UpdateGridPosition(Vector3Int pos, GameGrid grid) {
		gridPosition = pos;
        UpdateRealPosition(grid.Grid2RealPos(gridPosition));
	}

	public virtual bool IsActive() {
		return gameObject.activeInHierarchy;
	}
	
	protected Vector3Int StepToPosition(Vector3Int pos, int speed) {
		return new Vector3Int(Mathf.Clamp(pos.x - gridPosition.x,  -speed, speed),
							  Mathf.Clamp(pos.y - gridPosition.y,  -speed, speed),
							  Mathf.Clamp(pos.z - gridPosition.z,  -speed, speed));
	}
	
	public void BumpTowards(Vector3Int target, GameGrid grid, float distanceScale = 5.0f) {
		StartCoroutine( spriteAnimator.SmoothBump(grid.Grid2RealPos(target), distanceScale) );
	}
			
	// move only if you can, return non-null if you can't move and there is a Component blocking you
	public Component AttemptGridMove(int xdir, int ydir, GameGrid grid, bool addlConditions = true) {
		Vector3Int endPos = gridPosition.GridPosInDirection(grid, new Vector2Int(xdir, ydir));
		Vector3 endpoint = grid.Grid2RealPos(endPos);

		if (grid.IsInBounds(endPos) && addlConditions) {
			var occupant = grid.OccupantAt(endPos);

			if (occupant == null) {
				// SUCCESS!
				MoveDirection(xdir, ydir, grid);
						
				if (spriteAnimator.isMoving) StopCoroutine(crtMovement);
				crtMovement = StartCoroutine( spriteAnimator.SmoothMovement(endpoint) );
			}
			return occupant;

		// No success, you're out of bounds
		} else {
			if (spriteAnimator.isMoving) StopCoroutine(crtMovement);
			crtMovement = StartCoroutine( spriteAnimator.SmoothBump(endpoint, 5.0f) );
		}

		return null;
	}
	
	private void MoveDirection(int xdir, int ydir, GameGrid grid) {
		Vector3Int endPos = gridPosition.GridPosInDirection(grid, new Vector2Int(xdir, ydir));

		// we can move: instantly update the grid w/ this info to block further inquiry
		// also, remove the ref to yourself and set occupancy to null. No two things can ever coexist, so this should be fine
		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(endPos, this);
		gridPosition = endPos;
	}
}
