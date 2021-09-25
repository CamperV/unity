using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static bool skipMovement = false;

	public float snappiness;
	public KeyCode debugKey;

	private Camera assignedCamera { get => gameObject.GetComponent<Camera>(); }

	private static Vector2 minBounds;
	private static Vector2 maxBounds;

	private Transform trackingTarget;
	private Vector3 trackingPosition {
		get {
			return (trackingTarget == null) ? transform.position : new Vector3(Mathf.Clamp(trackingTarget.position.x, minBounds.x, maxBounds.x),
							   												   Mathf.Clamp(trackingTarget.position.y, minBounds.y, maxBounds.y),
							   												   transform.position.z);
		}
	}
	private float trackingSize;
		
	public void RefitCamera(int heightInTiles) {
		trackingSize = heightInTiles / 1.5f;
		assignedCamera.orthographicSize = trackingSize;
	}
	
	public void SetBounds(Vector2 min, Vector2 max) {
		minBounds = min;
		maxBounds = max;
	}
	
	public void SetTracking(Transform _trackingTarget) {
		trackingTarget = _trackingTarget;
		transform.position = trackingPosition;
	}
	
	// performs constant tracking
	void LateUpdate() {
		float snapSpeed = Time.deltaTime*snappiness;
		float snapFactor = 0.01f;
		
		// update mouse wheel scale
		// only when holding our debug key, however
		if (Input.GetKey(debugKey)) {
			trackingSize *= 1f / (1f + Input.GetAxis("Mouse ScrollWheel"));
		}
		if (Mathf.Abs(assignedCamera.orthographicSize - trackingSize) > snapFactor) {
			
			assignedCamera.orthographicSize = Mathf.Lerp(assignedCamera.orthographicSize, trackingSize, snapSpeed);
		} else {
			assignedCamera.orthographicSize = trackingSize;
		}

		// if we've toggled a certain mode, disable animation/smooth movement
		if (skipMovement) {
			transform.position = trackingPosition;
			return;
		}

		// else, normal operation:
		// update tracking
		if (Vector3.Distance(transform.position, trackingPosition) > snapFactor) {
			transform.position = Vector3.Lerp(transform.position, trackingPosition, snapSpeed);
		} else {
			transform.position = trackingPosition;
		}
	}
}