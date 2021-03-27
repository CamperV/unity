using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public abstract class MovingObject : MonoBehaviour
{	
	public virtual float moveSpeed { get { return 1.0f; } }

	private Vector3Int _gridPosition;
	[HideInInspector] public Vector3Int gridPosition {
		get {
			return _gridPosition;
		}
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
	
	// child classes must specify which grid to travel on
	public abstract bool GridMove(int xdir, int ydir);
		
	public bool AttemptGridMove(int xdir, int ydir, GameGrid grid) {
		Component hitComponent;
		bool canMove = CrtMove(xdir, ydir, grid, out hitComponent);
		
		// but if you did...
		if(!canMove && hitComponent != null) {
			OnBlocked(hitComponent);
		}
		return canMove;
	}

	public void BumpTowards(Vector3Int target, GameGrid grid, float distanceScale = 5.0f) {
		StartCoroutine( SmoothBump(grid.Grid2RealPos(target), distanceScale) );
	}
	
	private bool CrtMove(int xdir, int ydir, GameGrid grid, out Component occupant) {
		// need to always be a cell/Tile coordinate
		Vector3Int endTile = gridPosition.GridPosInDirection(grid, new Vector2Int(xdir, ydir));
		Vector3 endpoint = grid.Grid2RealPos(endTile);

		// first check if you're even in bounds, THEN get the occupant
		occupant = null;
		if (grid.IsInBounds(endTile)) {
			occupant = grid.OccupantAt(endTile);
			
			// no collisions
			if (occupant == null) {
				// we can move: instantly update the grid w/ this info to block further inquiry
				// also, remove the ref to yourself and set occupancy to null. No two things can ever coexist, so this should be fine
				grid.UpdateOccupantAt(gridPosition, null);
				grid.UpdateOccupantAt(endTile, this);
				gridPosition = endTile;
				
				// always interrupt a moving crt to change the destination of the SmoothMovement slide
				if (crtMovingFlag) StopCoroutine(crtMovement);
				crtMovement = StartCoroutine( SmoothMovement(endpoint) );
				return true;
			}
		}

		// Can't move?
		// always interrupt a moving crt to change the destination of the SmoothMovement slide
		if (crtMovingFlag) StopCoroutine(crtMovement);
		crtMovement = StartCoroutine( SmoothBump(endpoint, 5.0f) );
		return false;
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		crtMovingFlag = true;

		// we want it to take X seconds to go over one tile
		float fixedTime = 0.10f;
		float timeStep = 0.0f;
		Vector3 startPos = transform.position;

		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / fixedTime);
			transform.position = Vector3.Lerp(startPos, endpoint, timeStep);
			yield return null;
		}
		
		// after the while loop is broken:
		transform.position = endpoint;
		crtMovingFlag = false;
	}
	
	// this coroutine performs a little 'bump' when you can't move
	protected IEnumerator SmoothBump(Vector3 endpoint, float distanceScale) {
		crtMovingFlag = true;

		// we want it to take X seconds to go over one tile
		float fixedTime = 0.05f;

		Vector3 startPos = transform.position;
		Vector3 peakPos = startPos + (endpoint - transform.position)/distanceScale;
		
		float timeStep = 0.0f;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / fixedTime);
			transform.position = Vector3.Lerp(startPos, peakPos, timeStep);			
			yield return null;
		}

		// now for the return journey
		timeStep = 0.0f;
		while (timeStep < 1.0f)  {
			timeStep += (Time.deltaTime / fixedTime);
			transform.position = Vector3.Lerp(peakPos, startPos, timeStep);			
			yield return null;
		}
		
		// after the while loop is broken:
		transform.position = startPos;
		crtMovingFlag = false;
	}

	protected IEnumerator SmoothMovementPath(MovingObjectPath path, GameGrid grid) {
		crtMovingFlag = true;
		GameManager.inst.tacticsManager.resizeLock = true;

		// we want it to take X seconds to go over one tile
		float fixedTimePerTile = 0.10f;
		//
		Vector3 realNextPos = transform.position;
		foreach (var nextPos in path.Unwind()) {
			realNextPos = grid.Grid2RealPos(nextPos);

			float timeStep = 0.0f;
			Vector3 startPos = transform.position;

			while (timeStep < 1.0f) {
				timeStep += (Time.deltaTime / fixedTimePerTile);
				transform.position = Vector3.Lerp(startPos, realNextPos, timeStep);
				yield return null;
			}
		}
		transform.position = realNextPos;

		crtMovingFlag = false;
		GameManager.inst.tacticsManager.resizeLock = false;
	}

	public IEnumerator ExecuteAfterMoving(Action VoidAction) {
		while (crtMovingFlag) {
			yield return null;
		}
		VoidAction();
	}
	
	public virtual void OnBlocked<T>(T component) where T : Component { return; }
	public virtual bool IsMoving() { return crtMovingFlag; }
}
