using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ReportScreenQuadrant : MonoBehaviour
{
	public enum ScreenQuadrant {
		LL,
		LR,
		UL,
		UR
	};

	public UnityEvent<ScreenQuadrant> UpdateScreenQuadrantEvent;
	private ScreenQuadrant currentQuadrant;
	public ScreenQuadrant CurrentQuadrant => currentQuadrant;
	private ScreenQuadrant previousQuadrant;

	void OnEnable() {
		EventManager.inst.inputController.MousePositionEvent += ReportQuadrant;
	}

	void OnDisable() {
		EventManager.inst.inputController.MousePositionEvent -= ReportQuadrant;
	}

	private void ReportQuadrant(Vector3 screenPosition) {
		previousQuadrant = currentQuadrant;
		currentQuadrant = GetQuadrant(screenPosition);

		if (previousQuadrant != currentQuadrant) 
			UpdateScreenQuadrantEvent?.Invoke(currentQuadrant);
	}

	public static ScreenQuadrant GetQuadrant(Vector3 screenPosition) {
		Vector2 midpoint = new Vector2(Screen.width/2.0f, Screen.height/2.0f);

		if (screenPosition.y <= midpoint.y)
			return (screenPosition.x <= midpoint.x) ? ScreenQuadrant.LL : ScreenQuadrant.LR;
		else
			return (screenPosition.x <= midpoint.x) ? ScreenQuadrant.UL : ScreenQuadrant.UR;
	}
}
