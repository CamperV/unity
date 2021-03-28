using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private static Transform trackingTarget;
	private static Transform prevTrackingTarget;
	private static Vector2 minBounds;
	private static Vector2 maxBounds;
	
	public static void RefitStaticCamera(Vector3 pos, int height) {
		Camera.main.transform.position = pos;
		Camera.main.orthographicSize = (float)height / 2.0f;
	}
	
	public static void RefitCamera(int height) {
		Camera.main.orthographicSize = (float)height / 1.5f;
	}
	
	public static void SetBounds(Vector2 min, Vector2 max) {
		minBounds = min;
		maxBounds = max;
	}
	
	public static void SetTracking(Transform toTrack) {
		trackingTarget = toTrack;
	}

	public static void ResetTracking() {
		if (prevTrackingTarget != null) {
			trackingTarget = prevTrackingTarget;
		}
	}
	
	// performs constant tracking
	void LateUpdate() {
		Vector3 pos = new Vector3(Mathf.Clamp(trackingTarget.position.x, minBounds.x, maxBounds.x),
								  Mathf.Clamp(trackingTarget.position.y, minBounds.y, maxBounds.y),
								  transform.position.z);
		
		transform.position = pos;
	}
}