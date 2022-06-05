using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchoredSlider : MonoBehaviour
{
	public Vector2 slideDimensions;

	[Range(1, 50)]
	public float snappiness;

	private Vector2 activePosition;
	private Vector2 inactivePosition;
	private RectTransform rectTransform;

	public bool active = false;
	public Vector2 Destination => (active) ? activePosition : inactivePosition;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}

	void Start() {
		if (active) {
			activePosition = rectTransform.anchoredPosition;
			inactivePosition = rectTransform.anchoredPosition + slideDimensions;
		} else {
			activePosition = rectTransform.anchoredPosition - slideDimensions;
			inactivePosition = rectTransform.anchoredPosition;
		}
	}

	void Update() {
		float dist = Vector2.Distance(rectTransform.anchoredPosition, Destination);
		if (dist > 0.1f) {
			rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, Destination, snappiness*Time.deltaTime);

		} else if (dist < 0.1f && dist > 0f) {
			rectTransform.anchoredPosition = Destination;
		}
	}

	public void SetActive(bool _active, bool teleportInactiveFirst = false) {
		active = _active;
		if (teleportInactiveFirst) {
 			rectTransform.anchoredPosition = inactivePosition;
		}
	}
}