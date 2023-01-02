using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMousePosition : MonoBehaviour
{
	private Vector3 currentMousePosition;
	private bool initialized = false;
	[SerializeField] private Vector2 staticOffset;

	[Range(0f, 50f)]
	[SerializeField] private float smooth;

	[Range(0f, 10f)]
	[SerializeField] private float snapMagnitude;
	
	void OnEnable() {
		EventManager.inst.inputController.MousePositionEvent += UpdateMousePosition;
	}

	void OnDisable() {
		EventManager.inst.inputController.MousePositionEvent -= UpdateMousePosition;
		initialized = false;
	}

	private void UpdateMousePosition(Vector3 screenPosition) {
		currentMousePosition = screenPosition;
		
		// if you've been un-inited, re-snap to the mouse position
		if (!initialized) {
			initialized = true;
			transform.position = GetWorldPosition(currentMousePosition);
		}
	}

	private Vector3 GetWorldPosition(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
		return new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
	}

	// always update the position event for listeners
	void Update() {
		if (!initialized) return;

		Vector3 newPosition = GetWorldPosition(currentMousePosition);
		// snap if too far away
		// if (smooth > 0f && (newPosition - transform.position).magnitude < snapMagnitude) {
		if (smooth > 0f) {
			transform.position = Vector3.Lerp(transform.position, newPosition, (smooth)*Time.deltaTime);
			Debug.Log($"new pos {transform.position}");
		} else {
			transform.position = newPosition;
		}
	}
}
