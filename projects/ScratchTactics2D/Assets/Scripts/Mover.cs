using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : MonoBehaviour
{	
	public LayerMask blockingLayer;
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rigidbody2D;
	private bool crtMovingFlag = false;
	private Coroutine crtMovement;
	
    protected virtual void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		rigidbody2D = GetComponent<Rigidbody2D>();
    }
	
	protected Vector3Int ClampVec(Vector3Int vec, int speed) {
		return new Vector3Int(Mathf.Clamp(vec.x,  -speed, speed),
							  Mathf.Clamp(vec.y,  -speed, speed),
							  Mathf.Clamp(vec.z,  -speed, speed));
	}
	
	protected bool GridMove(int xdir, int ydir, out RaycastHit2D block) {
		// need to always be a cell/Tile coordinate
		Vector3 startpoint = transform.position; // implicitly casts
		Vector3 endpoint   = GameManager.inst.worldGrid.GetTileInDirection(startpoint, new Vector3Int(xdir, ydir, 0));
		
		// casts a ray and sees if the boxCollider hits anything along the vector startpoint->endpoint
		boxCollider.enabled = false;
		block = Physics2D.Linecast(startpoint, endpoint, blockingLayer);
		boxCollider.enabled = true;
		
		if (block.transform == null) {
			
			// to get that sweet smoothness
			// always interrupt a moving crt to change the destination of the SmoothMovement slide
			if (crtMovingFlag) StopCoroutine(crtMovement);
			crtMovement = StartCoroutine(SmoothMovement(endpoint));
			
			// for rigid, snappy movement
			//rigidbody2D.MovePosition(endpoint); 
			return true;
		}

		// only if you can't move properly
		
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
			//speedFactor = (15.0f * (1.0f/(sqrRemainingDistance*sqrRemainingDistance)) * Time.deltaTime) + 0.10f;
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
		Vector3 peak = transform.position + (endpoint - transform.position)/5.0f;
		
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
		//rigidbody2D.MovePosition(origPosition);
		crtMovingFlag = false;
	}
	
	protected virtual void AttemptMove<T>(int xdir, int ydir) where T : Component {
		RaycastHit2D hit;
		bool canMove = GridMove(xdir, ydir, out hit);
		
		// ie. made no collision
		if (hit.transform == null) return;
		
		// but if you did...
		T hitComponent = hit.transform.GetComponent<T>(); // this gets the thing that hit it?!
		if(!canMove && hitComponent != null) {
			OnBlocked(hitComponent);
		}
	}
	
	// abstract methods are inherently virtual
	protected abstract void OnBlocked<T>(T component) where T : Component;
}
