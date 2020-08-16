using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour {
	
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rigidbody2D;
	private float inverseMoveTime;
	
    protected virtual void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		rigidbody2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
    }
	
	protected bool Move(int xdir, int ydir, out RaycastHit2D block) {
		Vector2 startpoint = transform.position; // implicitly casts
		Vector2 endpoint   = startpoint + new Vector2(xdir, ydir);
		Debug.Log("Moving from "+ startpoint + " to " + endpoint + ", move vector " + xdir + ", " + ydir);
		// casts a ray and sees if the boxCollider hits anything along the vector startpoint->endpoint
		boxCollider.enabled = false;
		block = Physics2D.Linecast(startpoint, endpoint, blockingLayer);
		boxCollider.enabled = true;
		
		if (block.transform == null) {
			//StartCoroutine(SmoothMovement(endpoint));
			rigidbody2D.MovePosition(endpoint);
			return true;
		}
		
		// only if you can't move properly
		return false;
	}
	
	// this is like a Python-generator: Coroutine
	protected IEnumerator SmoothMovement(Vector3 endpoint) {
		float sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
		
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, endpoint, inverseMoveTime * Time.deltaTime);
			rigidbody2D.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
	}
	
	protected virtual void AttemptMove<T>(int xdir, int ydir) where T : Component {
		RaycastHit2D hit;
		bool canMove = Move(xdir, ydir, out hit);
		
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
