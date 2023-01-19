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

	private Vector3 trackingPosition;
	private Transform trackingTarget;
	public bool cameraLock = true;

	public Vector2 cameraSpeed;
	private Vector3 movementVector = Vector3.zero;

	// radius to give fudge factor to traack Lerping
	public float trackingTolerance;	
	public float lerpSpeed;

	private new Camera camera;
	private float zoomLevel;
	public float zoomSpeed;

	public float minOrthographicSize;
	public float maxOrthographicSize;

	// ZOOM WHEEL HARDWARE SPECIFIC
	private readonly float scrollTick = 120f;

	[SerializeField] private EventManager eventManager;

	void Awake() {
		camera = GetComponent<Camera>();
		cameraLock = true;
	}

	void Start() {
		trackingPosition = transform.position;
		
		// initial bounds calculation
		CalculateTilemapBounds();

		// init for scrolling
		zoomLevel = camera.orthographicSize;
	}

	public static void FocusActiveCameraOn(Vector3 focalPoint) {
		CameraManager activeCM = Camera.main.GetComponent<CameraManager>();
		if (activeCM.cameraLock) activeCM.trackingPosition = focalPoint;
	}

	public void UpdateMovementVector(Vector2 directionalInput) {
		movementVector = new Vector3(cameraSpeed.x*directionalInput.x, cameraSpeed.y*directionalInput.y, 0);
	}

	public void CalculateZoomUpdate() {
		// MOUSEINPUT FOR SCROLLING SEEMS TO BE BROKEN IN UNITY
		// use this in the meantime:
		if (eventManager.inputController.gameObject.activeInHierarchy) {
			Vector2 zoomVec = Mouse.current.scroll.ReadValue();
			if (zoomVec.y != 0) UpdateZoomLevel(zoomVec);
		}

		// update this each frame, but don't update the input each frame
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomLevel, Time.deltaTime*zoomSpeed);
	}

	public void UpdateZoomLevel(Vector2 mouseScrollInput) {
		float newZoom = mouseScrollInput.y / scrollTick;
		zoomLevel = Mathf.Clamp(camera.orthographicSize - newZoom, minOrthographicSize, maxOrthographicSize);
	}

	public void LateUpdate() {
		// CheckZoomUpdate();

		// recalculate every frame for zoom input
		// could do this on demand, but if we have a tracking target, we're doing this anyway
		CalculateTilemapBounds();

		// if we have a tracking target, make a smaller box around it so that it is "focused"
		// trackingTargets are acquired via Events + the UnitControllers
		if (trackingTarget != null && cameraLock) {
			Vector3 minTrackingBox = trackingTarget.position - OrthographicBounds();
			Vector3 maxTrackingBox = trackingTarget.position + OrthographicBounds();

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

		transform.position = Vector3.Lerp(transform.position, trackingPosition, Time.deltaTime*lerpSpeed);
	}

	public void AcquireTrackingTarget(Transform selection) {
		// we update this every frame because the playerUnitController might 
		// have a currentSelection, which we use to bound the camera additionally
		// (such that the camera must be within a certain distance from the currentSelection)
		if (selection != null) {
			trackingTarget = selection;

		// use the assigned tilemap to find the bounds
		} else {
			CalculateTilemapBounds();
			trackingTarget = null;
		}
	}


	public void FocusUnit(Unit unit) => FocusTarget(unit?.transform ?? null);
	public void FocusTarget(Transform selection) {
		if (selection == null) return;
		
		// dont' actually track this target - move the camera if necessary, but then let it be free
		trackingTarget = null;

		trackingPosition = new Vector3(
			Mathf.Clamp(selection.position.x, minBounds.x, maxBounds.x),
			Mathf.Clamp(selection.position.y, minBounds.y, maxBounds.y),
			transform.position.z
		);
	}

	public void ToggleCameraLock() {
		CalculateTilemapBounds();
		// trackingTarget = null;
		//
		cameraLock = !cameraLock;
	}

	private void CalculateTilemapBounds() {
		Vector3 _minBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.min) + OrthographicBounds();
		Vector3 _maxBounds = fitToTilemap.LocalToWorld(fitToTilemap.localBounds.max) - OrthographicBounds();

		// need to determine whether the bounds are overlapping with one another;
		// ie, if orthographic.x > 1/2 of the tilemap extents, we max out at "tilemap extents"
		// constrain to within the center
		// could probably do this via Clamping min/maxBounds to the center of 
		// fitToTilemap.LocalToWorld, but too lazy
		if (_minBounds.x > _maxBounds.x) {
			float swp = _minBounds.x;
			_minBounds.x = _maxBounds.x;
			_maxBounds.x = swp;
		}
		if (_minBounds.y > _maxBounds.y) {
			float swp = _minBounds.y;
			_minBounds.y = _maxBounds.y;
			_maxBounds.y = swp;
		}

		minBounds = _minBounds;
		maxBounds = _maxBounds;
	}

	private Vector3 OrthographicBounds() {
		// return Vector3.zero;
		// the bounds moved "inwards"
		return new Vector3(
			(camera.orthographicSize*camera.aspect) - 2,
			(camera.orthographicSize) - 2,
			0
		);
	}
}