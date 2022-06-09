using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
	public Tilemap fitToTilemap;
	private Vector3 minBounds;
	private Vector3 maxBounds;

	// THIS NEEDS TO BE SCALED TO ORTHOGRAPHIC SIZE
	// private readonly Vector3 fixedBoxOffset = new Vector3(3, 3, 0);
	private Vector3 fixedBoxOffset => new Vector3(-2 + camera.orthographicSize, -2 + camera.orthographicSize, 0);

	private Vector3 trackingPosition;
	private Transform trackingTarget;
	public bool cameraLock = true;

	public Vector2 cameraSpeed;
	private Vector3 movementVector = Vector3.zero;

	private new Camera camera;
	private float zoomLevel;
	public float zoomSpeed;

	public float minOrthographicSize;
	public float maxOrthographicSize;

	// ZOOM WHEEL HARDWARE SPECIFIC
	private readonly float scrollTick = 120f;

	private EventManager _cachedEventManager;

	void Awake() {
		camera = GetComponent<Camera>();
		cameraLock = true;
	}

	void Start() {
		trackingPosition = transform.position;
		
		// initial bounds calculation
		SetDefaultBounds();

		// init for scrolling
		zoomLevel = camera.orthographicSize;

		// cache the event manager so that we don't look it up every frame
		_cachedEventManager = GameObject.Find("Battle").GetComponent<EventManager>();
	}

	public static void FocusActiveCameraOn(Vector3 focalPoint) {
		CameraManager activeCM = Camera.main.GetComponent<CameraManager>();
		if (activeCM.cameraLock) activeCM.trackingPosition = focalPoint;
	}

	public void UpdateMovementVector(Vector2 directionalInput) {
		movementVector = new Vector3(cameraSpeed.x*directionalInput.x, cameraSpeed.y*directionalInput.y, 0);
	}

	public void UpdateZoomLevel(Vector2 mouseScrollInput) {
		float newZoom = mouseScrollInput.y / scrollTick;
		zoomLevel = Mathf.Clamp(camera.orthographicSize - newZoom, minOrthographicSize, maxOrthographicSize);
	}

	public void LateUpdate() {
		// MOUSEINPUT FOR SCROLLING SEEMS TO BE BROKEN IN UNITY
		// use this in the meantime:
		if (_cachedEventManager.inputController.gameObject.activeInHierarchy) {
			Vector2 zoomVec = Mouse.current.scroll.ReadValue();
			if (zoomVec.y != 0) UpdateZoomLevel(zoomVec);
		}


		// update this each frame, but don't update the input each frame
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomLevel, Time.deltaTime*zoomSpeed);

		// if we have a tracking target, make a smaller box around it so that it is "focused"
		// trackingTargets are acquired via Events + the UnitControllers
		if (trackingTarget != null && cameraLock) {
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

		
			Vector3 bl = minBounds;
			Vector3 br = new Vector3(maxBounds.x, minBounds.y, 0);
			Vector3 tl = new Vector3(minBounds.x, maxBounds.y, 0);
			Vector3 tr = maxBounds;
			Debug.DrawLine(bl, br, Color.green, Time.deltaTime, false);
			Debug.DrawLine(br, tr, Color.green, Time.deltaTime, false);
			Debug.DrawLine(tr, tl, Color.green, Time.deltaTime, false);
			Debug.DrawLine(tl, bl, Color.green, Time.deltaTime, false);
		}
		
		// move the tracking position based on movement and clamp it into bounds
		trackingPosition += Time.deltaTime*movementVector;
		trackingPosition = new Vector3(
			Mathf.Clamp(trackingPosition.x, minBounds.x, maxBounds.x),
			Mathf.Clamp(trackingPosition.y, minBounds.y, maxBounds.y),
			transform.position.z
		);

		//
		transform.position = Vector3.Lerp(transform.position, trackingPosition, Time.deltaTime*7f);
	}

	public void AcquireTrackingTarget(Transform selection) {
		// we update this every frame because the playerUnitController might 
		// have a currentSelection, which we use to bound the camera additionally
		// (such that the camera must be within a certain distance from the currentSelection)
		if (selection != null) {
			trackingTarget = selection;

		// use the assigned tilemap to find the bounds
		} else {
			SetDefaultBounds();
			trackingTarget = null;
		}
	}

	public void ToggleCameraLock() {
		SetDefaultBounds();
		// trackingTarget = null;
		//
		cameraLock = !cameraLock;
	}

	private void SetDefaultBounds() {
		minBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.min) + fixedBoxOffset;
		maxBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.max) - fixedBoxOffset;
	}
}