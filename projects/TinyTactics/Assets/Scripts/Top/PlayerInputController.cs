using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{	
	// accessible delegates
	public delegate void MouseClick(Vector3 screenPosition);
    public event MouseClick LeftMouseClickEvent;
	public event MouseClick RightMouseClickEvent;

	public delegate void MousePosition(Vector3 screenPosition);
	public event MousePosition MousePositionEvent;
	//

	private MouseInput mouseInput;

	void Awake() {
		mouseInput = new MouseInput();
	}
	
	void OnEnable() => mouseInput.Enable();
	void OnDisable() => mouseInput.Disable();

	void Start() {
		// register to InputActions
		mouseInput.MouseActionMap.LeftMouseClick.performed += ctx => OnLeftMouseClick();
		mouseInput.MouseActionMap.RightMouseClick.performed += ctx => OnRightMouseClick();
	}

	// always update the position event for listeners
	void Update() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		MousePositionEvent(mousePosition);
	}

	public void OnLeftMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseClickEvent(mousePosition);
	}

	public void OnRightMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		RightMouseClickEvent(mousePosition);
	}
}