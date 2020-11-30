using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{	
	[HideInInspector] public Vector3Int gridPosition { get; protected set; }
	
	protected Rigidbody2D rigidbody2D;
	
	protected bool crtMovingFlag = false;
	protected Coroutine crtMovement;
	
    protected virtual void Start() {
		rigidbody2D = GetComponent<Rigidbody2D>();
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
		float snapFactor = 0.01f;
		
		float speedFactor;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > snapFactor) {	
			speedFactor = (15.0f * (1.0f/sqrRemainingDistance) * Time.deltaTime) + 0.10f;

			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, endpoint, speedFactor);
			rigidbody2D.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		rigidbody2D.MovePosition(endpoint);
		crtMovingFlag = false;
	}
	
	// this coroutine performs a little 'bump' when you can't move
	protected IEnumerator SmoothBump(Vector3 endpoint) {	
		float speedFactor = 0.10f;
		
		Vector3 origPosition = transform.position;
		Vector3 peak = origPosition + (endpoint - transform.position)/5.0f;
		
		// while these are the same now, they only need to be initialized the same
		float sqrRemainingDistance = (transform.position - peak).sqrMagnitude;
		float sqrReturnDistance    = (peak - transform.position).sqrMagnitude;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, peak, speedFactor);
			rigidbody2D.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - peak).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		while (sqrReturnDistance > float.Epsilon) {
			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, origPosition, speedFactor);
			rigidbody2D.MovePosition(newPos);
			sqrReturnDistance = (origPosition - transform.position).sqrMagnitude;

			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		rigidbody2D.MovePosition(origPosition);
		crtMovingFlag = false;
	}

	protected IEnumerator SmoothMovementPath(MovingObjectPath path, GameGrid grid) {
		float snapFactor = 0.01f;
		float fixedDivisions = 10.0f;

		Vector3 realPos;
		foreach (var nextPos in path.Unwind()) {
			realPos = grid.Grid2RealPos(nextPos);
			
			// we want it to take X seconds to go over one tile
			float sqrRemainingDistance = (transform.position - realPos).sqrMagnitude;
			float distanceStep = sqrRemainingDistance / fixedDivisions;

			while (sqrRemainingDistance > snapFactor*snapFactor) {
				transform.position = Vector3.MoveTowards(transform.position, realPos, distanceStep);
				sqrRemainingDistance = (transform.position - realPos).sqrMagnitude;
				yield return null;
			}
		}
	}
	
	public virtual void OnBlocked<T>(T component) where T : Component { return; }
}
