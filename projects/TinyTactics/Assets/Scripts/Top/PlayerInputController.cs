using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{	
	// accessible delegates
	public delegate void MouseClick(Vector3 screenPosition);
    public event MouseClick LeftMouseClickEvent;
	public event MouseClick CtrlClickEvent;
	public event MouseClick RightMouseClickEvent;
	public event MouseClick MiddleMouseClickEvent;

	public delegate void MouseHold(Vector3 screenPosition);
	public event MouseHold LeftMouseHoldStartEvent;
	public event MouseHold LeftMouseHoldEndEvent;

	public delegate void MousePosition(Vector3 screenPosition);
	public event MousePosition MousePositionEvent;

	public delegate void MouseScroll(Vector2 scrollAxis);
	public event MouseScroll MouseScrollEvent;

	public delegate void ButtonDown();
	public event ButtonDown MainInteractButtonEvent;
	public event ButtonDown SelectNextUnitEvent;
	public event ButtonDown NextWeaponEvent;
	public event ButtonDown PrevWeaponEvent;

	public delegate void QuickBarSlotSelect(int slot);
	public event QuickBarSlotSelect QuickBarSlotSelectEvent;

	public delegate void DirectionalInput(Vector2 v);
	public event DirectionalInput DirectionalInputEvent;
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
		mouseInput.MouseActionMap.LeftMouseClick_Modified.performed += ctx => OnCtrlClickEvent();
		mouseInput.MouseActionMap.RightMouseClick.performed += ctx => OnRightMouseClick();
		mouseInput.MouseActionMap.MiddleMouseClick.performed += ctx => OnMiddleMouseClick();

		mouseInput.MouseActionMap.LeftMouseHold.performed += ctx => OnLeftMouseHoldStart();
		mouseInput.MouseActionMap.LeftMouseHold.canceled += ctx => OnLeftMouseHoldEnd();

		mouseInput.MouseActionMap.MouseScroll.performed += OnMouseScroll;

		keyboardInput.KeyboardActionMap.MainInteractButton.performed += ctx => OnMainInteractButton();
		keyboardInput.KeyboardActionMap.SelectNextUnit.performed += ctx => SelectNextUnitEvent?.Invoke();

		keyboardInput.KeyboardActionMap.Axis.performed += ctx => OnAxisMovement();
		keyboardInput.KeyboardActionMap.Axis.canceled += ctx => OnAxisMovement();

		keyboardInput.KeyboardActionMap.QuickBar_0.performed += ctx => OnQuickBar(0);
		keyboardInput.KeyboardActionMap.QuickBar_1.performed += ctx => OnQuickBar(1);
		keyboardInput.KeyboardActionMap.QuickBar_2.performed += ctx => OnQuickBar(2);
		keyboardInput.KeyboardActionMap.QuickBar_3.performed += ctx => OnQuickBar(3);
		keyboardInput.KeyboardActionMap.QuickBar_4.performed += ctx => OnQuickBar(4);
		keyboardInput.KeyboardActionMap.QuickBar_5.performed += ctx => OnQuickBar(5);
		keyboardInput.KeyboardActionMap.QuickBar_6.performed += ctx => OnQuickBar(6);
		keyboardInput.KeyboardActionMap.QuickBar_7.performed += ctx => OnQuickBar(7);
		keyboardInput.KeyboardActionMap.QuickBar_8.performed += ctx => OnQuickBar(8);
		keyboardInput.KeyboardActionMap.QuickBar_9.performed += ctx => OnQuickBar(9);

		keyboardInput.KeyboardActionMap.NextWeapon.performed += ctx => NextWeaponEvent?.Invoke();
		keyboardInput.KeyboardActionMap.PrevWeapon.performed += ctx => PrevWeaponEvent?.Invoke();
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

	public void OnCtrlClickEvent() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		CtrlClickEvent?.Invoke(mousePosition);
	}

	public void OnRightMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		RightMouseClickEvent?.Invoke(mousePosition);
	}

	public void OnMiddleMouseClick() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		MiddleMouseClickEvent?.Invoke(mousePosition);
	}

	public void OnLeftMouseHoldStart() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseHoldStartEvent?.Invoke(mousePosition);
	}

	public void OnLeftMouseHoldEnd() {
		Vector2 mousePosition = mouseInput.MouseActionMap.MousePosition.ReadValue<Vector2>();
		LeftMouseHoldEndEvent?.Invoke(mousePosition);
	}

	public void OnMouseScroll(InputAction.CallbackContext context) {
		Vector2 scrollVec = context.ReadValue<Vector2>();
		Debug.Log($"received new mousescrollinput {scrollVec}");
		MouseScrollEvent?.Invoke(scrollVec);
	}

	// right now, this is Space
	public void OnMainInteractButton() {
		MainInteractButtonEvent?.Invoke();
	}

	public void OnAxisMovement() {
		Vector2 inputVector = keyboardInput.KeyboardActionMap.Axis.ReadValue<Vector2>();
		DirectionalInputEvent?.Invoke(inputVector);
	}

	public void OnQuickBar(int slot) {
		QuickBarSlotSelectEvent?.Invoke(slot);
	}
}
