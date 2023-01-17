using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeepInViewport_TopLeft : MonoBehaviour
{
	private RectTransform rootCanvasRect;
	private RectTransform rectTransform;

	// corners on a rect
	private Vector2 minBounds;
	private Vector2 maxBounds;

	void Awake() {
		rootCanvasRect = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
		rectTransform = GetComponent<RectTransform>();
	}

	// this works for a middle-aligned canvas, and a top-left aligned target
	// not general... oh well whoopsie I guess I'll just do less work :^)
	void Start() {
		float halfX = rootCanvasRect.sizeDelta.x/2f;
		float halfY = rootCanvasRect.sizeDelta.y/2f;

		// canvas bounds
		minBounds = new Vector2(-halfX, -halfY);
		maxBounds = new Vector2(halfX, halfY);
	}

	void Update() {
		float clampedX = Mathf.Clamp(rectTransform.anchoredPosition.x, minBounds.x, maxBounds.x - rectTransform.sizeDelta.x);
		float clampedY = Mathf.Clamp(rectTransform.anchoredPosition.y, minBounds.y + rectTransform.sizeDelta.y, maxBounds.y);
		rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
	}
}
