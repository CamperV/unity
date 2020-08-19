using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour {
	
	public WorldGrid worldGrid; // i hate this
	public LayerMask blockingLayer;
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rigidbody2D;
	private bool crtMovingFlag = false;
	private Coroutine crtMovement;
	
    protected virtual void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		rigidbody2D = GetComponent<Rigidbody2D>();
    }
	
	public void SetWorld(WorldGrid wg) {
		worldGrid = wg;
	}
	
	protected bool GridMove(int xdir, int ydir, out RaycastHit2D block) {
		// need to always be a cell/Tile coordinate
		Vector3 startpoint = transform.position; // implicitly casts
		Vector3 endpoint   = worldGrid.GetTileInDirection(startpoint, new Vector3Int(xdir, ydir, 0));
		
		Debug.Log("Moving from "+ startpoint + " to " + endpoint + ", move vector " + xdir + ", " + ydir);
		// casts a ray and sees if the boxCollider hits anything along the vector startpoint->endpoint
		boxCollider.enabled = false;
		block = Physics2D.Linecast(startpoint, endpoint, blockingLayer);
		boxCollider.enabled = true;
		
		if (block.transform == null) {
			
			// to get that sweet smoothness
			if (crtMovingFlag) {
				Debug.Log("stopped");
				StopCoroutine(crtMovement);
			}
			crtMovement = StartCoroutine(SmoothMovement(endpoint));
			
			// for rigid, snappy movement
			//rigidbody2D.MovePosition(endpoint); 
			return true;
		}
		
		// only if you can't move properly
		return false;
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		float sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
		float snapFactor = 0.01f;
		float speedFactor;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > snapFactor) {
			speedFactor = (150.0f * (1.0f/(sqrRemainingDistance*sqrRemainingDistance)) * Time.deltaTime) + 0.10f;

			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, endpoint, speedFactor);
			rigidbody2D.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		Debug.Log("snapped " + transform.position + ", " + endpoint);
		rigidbody2D.MovePosition(endpoint);
		crtMovingFlag = false;
	}
	
	protected virtual void AttemptMove<T>(int xdir, int ydir) where T : Component {
		RaycastHit2D hit;
		bool canMove = GridMove(xdir, ydir, out hit);
		
		// ie. made no collision
		if (hit.transform == null) {
			return;
		}
		
		// but if you did...
		T hitComponent = hit.transform.GetComponent<T>(); // this gets the thing that hit it?!
		if(!canMove && hitComponent != null) {
			OnBlocked(hitComponent);
		}
	}
	
	protected abstract void OnBlocked<T>(T component) where T : Component;
}
