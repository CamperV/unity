using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{	
	public LayerMask blockingLayer;
	public Vector3Int gridPosition { get; protected set; }
	public float moveDelayTime = .005f;	// in units of WaitForSeconds();

	private Rigidbody2D rigidbody2D;
	private bool crtMovingFlag = false;
	private Coroutine crtMovement;
	
    protected virtual void Start() {
		rigidbody2D = GetComponent<Rigidbody2D>();
    }
	
	protected Vector3Int SpeedVec(Vector3Int vec, int speed) {
		return new Vector3Int(Mathf.Clamp(vec.x,  -speed, speed),
							  Mathf.Clamp(vec.y,  -speed, speed),
							  Mathf.Clamp(vec.z,  -speed, speed));
	}
	
	protected Vector3Int ToPosition(Vector3Int pos, int speed) {
		return new Vector3Int(Mathf.Clamp(pos.x - gridPosition.x,  -speed, speed),
							  Mathf.Clamp(pos.y - gridPosition.y,  -speed, speed),
							  Mathf.Clamp(pos.z - gridPosition.z,  -speed, speed));
	}
	
	protected bool GridMove(int xdir, int ydir, out Component occupant) {
		// need to always be a cell/Tile coordinate
		Vector3Int endTile   = gridPosition + new Vector3Int(xdir, ydir, 0);
		Vector3 endpoint = GameManager.inst.worldGrid.Grid2RealPos(endTile);

		// first check if you're even in bounds, THEN get the occupant
		occupant = null;
		if (GameManager.inst.worldGrid.IsInBounds(endTile)) {
			occupant = GameManager.inst.worldGrid.OccupantAt(endTile);
			
			// no collisions
			if (occupant == null) {
				// we can move: instantly update the worldGrid w/ this info to block further inquiry
				// also, remove the ref to yourself and set occupancy to null. No two things can ever coexist, so this should be fine
				GameManager.inst.worldGrid.UpdateOccupantAt(gridPosition, null);
				GameManager.inst.worldGrid.UpdateOccupantAt(endTile, this);
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
	
	protected virtual void AttemptGridMove(int xdir, int ydir) {
		Component hitComponent;
		bool canMove = GridMove(xdir, ydir, out hitComponent);
		
		// but if you did...
		if(!canMove && hitComponent != null) {
			OnBlocked(hitComponent);
		}
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		float sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
		float snapFactor = 0.01f;
		float speedFactor;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > snapFactor) {
			speedFactor = (15.0f * (1.0f/sqrRemainingDistance) * Time.deltaTime) + 0.10f;

			//Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, endpoint, 1f/moveDelayTime * (Time.deltaTime*100));
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
		
		Vector3 origPosition = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
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
	
	// abstract methods are inherently virtual
	protected abstract void OnBlocked<T>(T component) where T : Component;
	
	// neighbors are defined as adjacent squares in cardinal directions
	/*public List<Vector3Int> GetNeighbors() {
		List<Vector3Int> cardinal = new List<Vector3Int> {
			gridPosition + new Vector3Int( 0,  1, 0), // N
			gridPosition + new Vector3Int( 0, -1, 0), // S
			gridPosition + new Vector3Int(-1,  0, 0), // E
			gridPosition + new Vector3Int( 1,  0, 0)  // W
		};
		// here, we'd loop through and determine which, if any, are valid
		
		return cardinal;
	}*/
}
