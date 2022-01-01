using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	// public float snappiness;

	// private Camera assignedCamera => GetComponent<Camera>();

	// private static Vector2 minBounds;
	// private static Vector2 maxBounds;

	// private Transform trackingTarget;
	// private Vector3 trackingPosition {
	// 	get {
	// 		return (trackingTarget == null) ? transform.position : new Vector3(Mathf.Clamp(trackingTarget.position.x, minBounds.x, maxBounds.x),
	// 						   												   Mathf.Clamp(trackingTarget.position.y, minBounds.y, maxBounds.y),
	// 						   												   transform.position.z);
	// 	}
	// }
	// private float trackingSize;

	// void Start() {
	// 	minBounds = new Vector2(0, 0);
	// 	maxBounds = new Vector2(10, 10);
	// 	RefitCamera(10);
	// }

	private Vector3 trackingPosition;

	public Vector2 cameraSpeed;
	private Vector3 movementVector = Vector3.zero;

	void Start() {
		trackingPosition = transform.position;
	}

	public void UpdateMovementVector(Vector2 directionalInput) {
		movementVector = new Vector3(cameraSpeed.x*directionalInput.x, cameraSpeed.y*directionalInput.y, 0);
	}

	public void Update() {
		trackingPosition += Time.deltaTime*movementVector;
		//
		transform.position = Vector3.Lerp(transform.position, trackingPosition, Time.deltaTime*6f);
	}
		
	// public void RefitCamera(int heightInTiles) {
	// 	trackingSize = heightInTiles / 1.5f;
	// 	assignedCamera.orthographicSize = trackingSize;
	// }
	
	// public void SetTracking(Transform _trackingTarget) {
	// 	trackingTarget = _trackingTarget;
	// 	transform.position = trackingPosition;
	// }
	
	// // performs constant tracking
	// void LateUpdate() {
	// 	float snapSpeed = Time.deltaTime*snappiness;
	// 	float snapFactor = 0.01f;
		
	// 	if (Mathf.Abs(assignedCamera.orthographicSize - trackingSize) > snapFactor) {
	// 		assignedCamera.orthographicSize = Mathf.Lerp(assignedCamera.orthographicSize, trackingSize, snapSpeed);
	// 	} else {
	// 		assignedCamera.orthographicSize = trackingSize;
	// 	}

	// 	// else, normal operation:
	// 	// update tracking
	// 	if (Vector3.Distance(transform.position, trackingPosition) > snapFactor) {
	// 		transform.position = Vector3.Lerp(transform.position, trackingPosition, snapSpeed);
	// 	} else {
	// 		transform.position = trackingPosition;
	// 	}
	// }
}