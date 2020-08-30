using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{	
	public LayerMask blockingLayer;
	public Vector3Int gridPosition { get; protected set; }
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rigidbody2D;
	private bool crtMovingFlag = false;
	private Coroutine crtMovement;
	
    protected virtual void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		rigidbody2D = GetComponent<Rigidbody2D>();
    }
	
	protected Vector3Int SpeedVec(Vector3Int vec, int speed) {
		return new Vector3Int(Mathf.Clamp(vec.x,  -speed, speed),
							  Mathf.Clamp(vec.y,  -speed, speed),
							  Mathf.Clamp(vec.z,  -speed, speed));
	}
	
	protected bool GridMove(int xdir, int ydir, out Component occupant) {
		// need to always be a cell/Tile coordinate
		Vector3Int endTile   = gridPosition + new Vector3Int(xdir, ydir, 0);
		Vector3 endpoint = GameManager.inst.worldGrid.Grid2RealPos(endTile);

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

		// Can't move?
		// always interrupt a moving crt to change the destination of the SmoothMovement slide
		if (crtMovingFlag) StopCoroutine(crtMovement);
		crtMovement = StartCoroutine(SmoothBump(endpoint));
		return false;
	}
	
	protected virtual void AttemptGridMove(int xdir, int ydir) {
		Component hitComponent;
		bool canMove = GridMove(xdir, ydir, out hitComponent);
		/*
		// ie. made no collision
		if (hit.transform == null) return;
		
		// but if you did...
		if(!canMove && hitComponent != null) {
			OnBlocked(hitComponent);
		}*/
	}
	
	protected bool Move(int xdir, int ydir, out RaycastHit2D block) {
		// need to always be a cell/Tile coordinate
		Vector3 startpoint = transform.position; // implicitly casts
		Vector3 endpoint   = GameManager.inst.worldGrid.GetTileInDirection(startpoint, new Vector3Int(xdir, ydir, 0));
		
		// casts a ray and sees if the boxCollider hits anything along the vector startpoint->endpoint
		boxCollider.enabled = false;
		block = Physics2D.Linecast(startpoint, endpoint, blockingLayer);
		boxCollider.enabled = true;
		
		// no collisions
		if (block.transform == null) {
			// always interrupt a moving crt to change the destination of the SmoothMovement slide
			if (crtMovingFlag) StopCoroutine(crtMovement);
			crtMovement = StartCoroutine(SmoothMovement(endpoint));
			return true;
		}

		// Can't move?
		// always interrupt a moving crt to change the destination of the SmoothMovement slide
		if (crtMovingFlag) StopCoroutine(crtMovement);
		crtMovement = StartCoroutine(SmoothBump(endpoint));
		return false;
	}
	
	protected void AttemptMove<T>(int xdir, int ydir) where T : Component {
		RaycastHit2D hit;
		bool canMove = Move(xdir, ydir, out hit);
		
		// ie. made no collision
		if (hit.transform == null) return;
		
		// but if you did...
		T hitComponent = hit.transform.GetComponent<T>(); // this gets the thing that hit it?!
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
}
