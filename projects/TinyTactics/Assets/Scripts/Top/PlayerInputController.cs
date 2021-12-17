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

	public delegate void MouseHold(Vector3 screenPosition);
	public event MouseHold LeftMouseHoldStartEvent;
	public event MouseHold LeftMouseHoldEndEvent;

	public delegate void MousePosition(Vector3 screenPosition);
	public event MousePosition MousePositionEvent;

	public delegate void ButtonDown();
	public event ButtonDown MainInteractButtonEvent;
	//

	private MouseInput mouseInput;
	private KeyboardInput keyboardInput;

	void Awake() {
		mouseInput = new MouseInput();
		keyboardInput = new KeyboardInput();
	}
	
	void OnEnable() {
		mouseInput.Enable();
		keyboardInput.Enable();
	}
	void OnDisable() {
		mouseInput.Disable();
		keyboardInput.Disable();
	}

	void Start() {
		// register to InputActions
		mouseInput.MouseActionMap.LeftMouseClick.performed += ctx => OnLeftMouseClick();
		mouseInput.MouseActionMap.RightMouseClick.performed += ctx => OnRightMouseClick();

		mouseInput.MouseActionMap.LeftMouseHold.performed += ctx => OnLeftMouseHoldStart();
		mouseInput.MouseActionMap.LeftMouseHold.canceled += ctx => OnLeftMouseHoldEnd();

		keyboardInput.KeyboardActionMap.MainInteractButton.performed += ctx => OnMainInteractButton();
	}

	// always update the position event for listeners
	void Update() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		MousePositionEvent?.Invoke(mousePosition);
	}

	public void OnLeftMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseClickEvent?.Invoke(mousePosition);
	}

	public void OnRightMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		RightMouseClickEvent?.Invoke(mousePosition);
	}

	public void OnLeftMouseHoldStart() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseHoldStartEvent?.Invoke(mousePosition);
	}

	public void OnLeftMouseHoldEnd() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseHoldEndEvent?.Invoke(mousePosition);
	}

	// right now, this is Space
	public void OnMainInteractButton() {
		MainInteractButtonEvent?.Invoke();
	}
}
