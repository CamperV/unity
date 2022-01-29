using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPopUp : MonoBehaviour
{
	public EventManager eventManager;

	public void OnEnable() {
		eventManager.DisablePlayerInput();
		eventManager.EnableMenuInput();
        eventManager.menuInputController.RightMouseClickEvent += AnywhereDismiss;
	}

	public void OnDisable() {
		eventManager.EnablePlayerInput();
        eventManager.menuInputController.RightMouseClickEvent -= AnywhereDismiss;

		eventManager.DisableMenuInput();
	}

	private void AnywhereDismiss(Vector3 pos) {
		gameObject.SetActive(false);
	}
}
