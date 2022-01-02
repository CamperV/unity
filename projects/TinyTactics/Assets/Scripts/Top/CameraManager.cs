using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraManager : MonoBehaviour
{
	public Tilemap fitToTilemap;
	private Vector3 minBounds;
	private Vector3 maxBounds;
	private readonly Vector3 fixedBoxOffset = new Vector3(3, 3, 0);

	private Vector3 trackingPosition;
	private Transform trackingTarget;

	public Vector2 cameraSpeed;
	private Vector3 movementVector = Vector3.zero;

	void Start() {
		trackingPosition = transform.position;
		
		// initial bounds calculation
		SetDefaultBounds();
	}

	public void UpdateMovementVector(Vector2 directionalInput) {
		movementVector = new Vector3(cameraSpeed.x*directionalInput.x, cameraSpeed.y*directionalInput.y, 0);
	}

	public void LateUpdate() {

		// if we have a tracking target, make a smaller box around it so that it is "focused"
		// trackingTargets are acquired via Events + the UnitControllers
		if (trackingTarget != null) {
			SetDefaultBounds();

			// Vector3 minTrackingBox = trackingTarget.position - 0.5f*fitToTilemap.localBounds.extents;
			// Vector3 maxTrackingBox = trackingTarget.position + 0.5f*fitToTilemap.localBounds.extents;
			Vector3 minTrackingBox = trackingTarget.position - fixedBoxOffset;
			Vector3 maxTrackingBox = trackingTarget.position + fixedBoxOffset;

			minBounds = new Vector3(
				Mathf.Max(minTrackingBox.x, minBounds.x),
				Mathf.Max(minTrackingBox.y, minBounds.x),
				Mathf.Max(minTrackingBox.z, minBounds.z)
			);
			maxBounds = new Vector3(
				Mathf.Min(maxTrackingBox.x, maxBounds.x),
				Mathf.Min(maxTrackingBox.y, maxBounds.x),
				Mathf.Min(maxTrackingBox.z, maxBounds.z)
			);
		}
		
		// move the tracking position based on movement and clamp it into bounds
		trackingPosition += Time.deltaTime*movementVector;
		trackingPosition = new Vector3(
			Mathf.Clamp(trackingPosition.x, minBounds.x, maxBounds.x),
			Mathf.Clamp(trackingPosition.y, minBounds.y, maxBounds.y),
			transform.position.z
		);

		//
		transform.position = Vector3.Lerp(transform.position, trackingPosition, Time.deltaTime*6f);
	}

	public void AcquireTrackingTarget(Unit selection) {
		// we update this every frame because the playerUnitController might 
		// have a currentSelection, which we use to bound the camera additionally
		// (such that the camera must be within a certain distance from the currentSelection)
		if (selection != null) {
			trackingTarget = selection.transform;

		// use the assigned tilemap to find the bounds
		} else {
			SetDefaultBounds();
			trackingTarget = null;
		}
	}

	private void SetDefaultBounds() {
		minBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.min) + fixedBoxOffset;
		maxBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.max) - fixedBoxOffset;
	}
}