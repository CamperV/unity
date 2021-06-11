using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public abstract class MovingObject : MonoBehaviour
{	
	// we want it to take X seconds to go over one tile
	public static float speedMultiplier = 1f;
	private float fixedTimePerTile { get => 0.10f / speedMultiplier; }

	protected Vector3Int _gridPosition;
	[HideInInspector] public virtual Vector3Int gridPosition {
		get => _gridPosition;
		protected set {
			_gridPosition = value;			
		}
	}
	
	protected bool crtMovingFlag = false;
	protected Coroutine crtMovement;
	public bool isMoving { get => crtMovingFlag; }

	public virtual bool IsActive() {
		return gameObject.activeInHierarchy;
	}
	
	protected Vector3Int ToPosition(Vector3Int pos, int speed) {
		return new Vector3Int(Mathf.Clamp(pos.x - gridPosition.x,  -speed, speed),
							  Mathf.Clamp(pos.y - gridPosition.y,  -speed, speed),
							  Mathf.Clamp(pos.z - gridPosition.z,  -speed, speed));
	}
	
	public void BumpTowards(Vector3Int target, GameGrid grid, float distanceScale = 5.0f) {
		StartCoroutine( SmoothBump(grid.Grid2RealPos(target), distanceScale) );
	}
			
	// move only if you can, return non-null if you can't move and there is a Component blocking you
	public Component AttemptGridMove(int xdir, int ydir, GameGrid grid, bool addlConditions = true) {
		Vector3Int endPos = gridPosition.GridPosInDirection(grid, new Vector2Int(xdir, ydir));
		Vector3 endpoint = grid.Grid2RealPos(endPos);

		if (grid.IsInBounds(endPos) && addlConditions) {
			var occupant = grid.OccupantAt(endPos);

			if (occupant == null) {
				// SUCCESS!
				Move(xdir, ydir, grid);
						
				if (crtMovingFlag) StopCoroutine(crtMovement);
				crtMovement = StartCoroutine( SmoothMovement(endpoint) );
			}
			return occupant;

		// No success, you're out of bounds
		} else {
			if (crtMovingFlag) StopCoroutine(crtMovement);
			crtMovement = StartCoroutine( SmoothBump(endpoint, 5.0f) );
		}

		return null;
	}
	
	private void Move(int xdir, int ydir, GameGrid grid) {
		Vector3Int endPos = gridPosition.GridPosInDirection(grid, new Vector2Int(xdir, ydir));

		// we can move: instantly update the grid w/ this info to block further inquiry
		// also, remove the ref to yourself and set occupancy to null. No two things can ever coexist, so this should be fine
		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(endPos, this);
		gridPosition = endPos;
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		crtMovingFlag = true;

		float timeStep = 0.0f;
		Vector3 startPos = transform.position;

		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / fixedTimePerTile);
			UpdateRealPosition(Vector3.Lerp(startPos, endpoint, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		UpdateRealPosition(endpoint);
		crtMovingFlag = false;
	}
	
	// this coroutine performs a little 'bump' when you can't move
	protected IEnumerator SmoothBump(Vector3 endpoint, float distanceScale) {
		crtMovingFlag = true;

		Vector3 startPos = transform.position;
		Vector3 peakPos = startPos + (endpoint - transform.position)/distanceScale;
		
		float timeStep = 0.0f;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			UpdateRealPosition(Vector3.Lerp(startPos, peakPos, timeStep));
			yield return null;
		}

		// now for the return journey
		timeStep = 0.0f;
		while (timeStep < 1.0f)  {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			UpdateRealPosition(Vector3.Lerp(peakPos, startPos, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		UpdateRealPosition(startPos);
		crtMovingFlag = false;
	}

	protected IEnumerator SmoothMovementPath(Path path, GameGrid grid) {
		crtMovingFlag = true;
		GameManager.inst.tacticsManager.resizeLock = true;

		Vector3 realNextPos = transform.position;
		foreach (var nextPos in path.Unwind()) {
			realNextPos = grid.Grid2RealPos(nextPos);

			float timeStep = 0.0f;
			Vector3 startPos = transform.position;

			while (timeStep < 1.0f) {
				timeStep += (Time.deltaTime / fixedTimePerTile);

				UpdateRealPosition(Vector3.Lerp(startPos, realNextPos, timeStep));
				yield return null;
			}
		}
		UpdateRealPosition(realNextPos);

		crtMovingFlag = false;
		GameManager.inst.tacticsManager.resizeLock = false;
	}

	public IEnumerator ExecuteAfterMoving(Action VoidAction) {
		while (crtMovingFlag) {
			yield return null;
		}
		VoidAction();
	}
	
	public virtual bool IsMoving() { return crtMovingFlag; }
	public virtual void UpdateRealPosition(Vector3 pos) {
		transform.position = pos;
	}
}
