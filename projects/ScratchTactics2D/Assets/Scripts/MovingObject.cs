using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{	
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
	
	private bool CrtMove(int xdir, int ydir, GameGrid grid, out Component occupant) {
		// need to always be a cell/Tile coordinate
		Vector3Int endTile = gridPosition + new Vector3Int(xdir, ydir, 0);
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
				crtMovement = StartCoroutine(SmoothMovement(endpoint));
				return true;
			}
		}

		// Can't move?
		// always interrupt a moving crt to change the destination of the SmoothMovement slide
		if (crtMovingFlag) StopCoroutine(crtMovement);
		crtMovement = StartCoroutine(SmoothBump(endpoint));
		return false;
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		float sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
		float speedFactor;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > float.Epsilon) {	
			speedFactor = (5.0f * (1.0f/sqrRemainingDistance) * Time.deltaTime);

			transform.position = Vector3.MoveTowards(transform.position, endpoint, speedFactor);
			sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		transform.position = endpoint;
		crtMovingFlag = false;
	}
	
	// this coroutine performs a little 'bump' when you can't move
	protected IEnumerator SmoothBump(Vector3 endpoint) {	
		float speedFactor = 0.055f;
		
		Vector3 origPosition = transform.position;
		Vector3 peak = origPosition + (endpoint - transform.position)/5.0f;
		
		// while these are the same now, they only need to be initialized the same
		float sqrRemainingDistance = (transform.position - peak).sqrMagnitude;
		float sqrReturnDistance    = (peak - transform.position).sqrMagnitude;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > float.Epsilon) {
			transform.position = Vector3.MoveTowards(transform.position, peak, speedFactor);
			sqrRemainingDistance = (transform.position - peak).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		while (sqrReturnDistance > float.Epsilon) {
			transform.position = Vector3.MoveTowards(transform.position, origPosition, speedFactor);
			sqrReturnDistance = (origPosition - transform.position).sqrMagnitude;

			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		transform.position = origPosition;
		crtMovingFlag = false;
	}

	protected IEnumerator SmoothMovementPath(MovingObjectPath path, GameGrid grid) {
		float fixedDivisions = 8.0f;

		crtMovingFlag = true;
		GameManager.inst.tacticsManager.scrollLock = true;
		//
		Vector3 realPos;
		foreach (var nextPos in path.Unwind()) {
			realPos = grid.Grid2RealPos(nextPos);
			
			// we want it to take X seconds to go over one tile
			float sqrRemainingDistance = (transform.position - realPos).sqrMagnitude;
			float distanceStep = ((transform.position - realPos).magnitude) / fixedDivisions;

			while (sqrRemainingDistance > float.Epsilon) {
				transform.position = Vector3.MoveTowards(transform.position, realPos, distanceStep);
				sqrRemainingDistance = (transform.position - realPos).sqrMagnitude;
				yield return null;
			}
		}
		crtMovingFlag = false;
		GameManager.inst.tacticsManager.scrollLock = false;
	}

	public IEnumerator ExecuteAfterMoving(Action voidAction) {
		while (crtMovingFlag) {
			yield return null;
		}
		voidAction();
	}
	
	public virtual void OnBlocked<T>(T component) where T : Component { return; }
	public virtual bool IsMoving() { return crtMovingFlag; }
}
