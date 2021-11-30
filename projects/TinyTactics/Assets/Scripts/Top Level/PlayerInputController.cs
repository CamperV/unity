using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{	
	// accessible delegates
	public delegate void ClickEvent(Vector3 screenPosition);
    public event ClickEvent MouseClickEvent;

	public delegate void PositionEvent(Vector3 screenPosition);
	public event PositionEvent MousePositionEvent;
	//

	private MouseInput mouseInput;

	void Awake() {
		mouseInput = new MouseInput();
	}
	
	void OnEnable() => mouseInput.Enable();
	void OnDisable() => mouseInput.Disable();

	void Start() {
		// register to InputActions
		mouseInput.MouseActionMap.MouseClick.performed += ctx => OnMouseClick();
	}

	// always update the position event for listeners
	void Update() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		MousePositionEvent(mousePosition);
	}

	public void OnMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		MouseClickEvent(mousePosition);
	}
}