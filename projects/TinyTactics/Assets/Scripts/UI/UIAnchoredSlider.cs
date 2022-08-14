using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchoredSlider : MonoBehaviour
{
	public Vector2 slideDimensions;
	public bool useHeight;
	public bool startInactive;

	[Range(1, 50)]
	public float snappiness;

	[Header("Activation Customization")]
	public bool slideOnEnable;
	public float slideOnEnableDelay;
	public UIAnchoredSlider cascadeAfter;

	[Header("Data")]
	public Vector2 activePosition;
	public Vector2 inactivePosition;

	private RectTransform rectTransform;
	private bool active;

	public bool InPosition => active && activePosition == rectTransform.anchoredPosition;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
		
		activePosition = rectTransform.anchoredPosition;

		Vector2 inactiveOffset = slideDimensions;
		if (useHeight) {
			// keep the direction, but make the magnitude dependent on the height of the object
			inactiveOffset = rectTransform.rect.height*inactiveOffset.normalized;
		}
		inactivePosition = rectTransform.anchoredPosition + inactiveOffset;
	}

	void OnEnable() {
		StartCoroutine( WaitForCascade() );
	}

	// this creates a cascading effect, where a parent slider must complete their slide
	// before this slider tries to slide.
	private IEnumerator WaitForCascade() {
		yield return new WaitUntil(() => cascadeAfter?.InPosition == true);
		if (slideOnEnableDelay > 0f)
			yield return new WaitForSeconds(slideOnEnableDelay);

		// then, do it
		SetActive(slideOnEnable, teleportInactiveFirst: startInactive);
	}

	void OnDisable() {
		SetActive(false);
	}

	void Update() {
		Vector2 destination = (active) ? activePosition : inactivePosition;
		float dist = Vector2.Distance(rectTransform.anchoredPosition, destination);

		if (dist > 0.1f) {
			rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, destination, snappiness*Time.deltaTime);

		} else if (dist < 0.1f && dist > 0f) {
			rectTransform.anchoredPosition = destination;
		}
	}

	public void SetActive(bool _active, bool teleportInactiveFirst = false) {
		active = _active;
		if (teleportInactiveFirst) {
 			rectTransform.anchoredPosition = inactivePosition;
		}
	}
}