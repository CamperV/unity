using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMousePosition : MonoBehaviour
{
    private MouseInput mouseInput;

	void Awake() {
		mouseInput = new MouseInput();
	}
	
	void OnEnable() {
		mouseInput.Enable();
	}
	void OnDisable() {
		mouseInput.Disable();
	}

	// always update the position event for listeners
	void Update() {
        Vector3 screenPosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
	}
}
