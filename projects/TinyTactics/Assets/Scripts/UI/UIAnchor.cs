using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchor : MonoBehaviour
{
	[SerializeField] public Vector3 anchor; // this is in world-space
	private Transform anchoredTransform;
	private Transform rotateTowards;
	private Transform scaleTowards;
	private float sizeDeltaOffset;
	private Vector2 originalSizeDelta;

	[SerializeField] public float minViewportBound = 0f;
	[SerializeField] public float maxViewportBound = 1f;
	
	[SerializeField] private Vector3 staticOffset;
	[SerializeField] private Vector3 staticOffset_RotationAnchor;
	[SerializeField] private bool smooth;

	public Vector3 ZeroZAnchor => new Vector3(anchor.x, anchor.y, 0f);

	void Start() {
		anchor = transform.position;
	}

	void Update() {
		// position update loop
		if (anchoredTransform != null) {
			Vector3 worldSpaceAnchor = anchoredTransform.position;
			anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z) + staticOffset;	
		}
		if (smooth) {
			transform.position = Vector3.Lerp(transform.position, anchor, 10f*Time.deltaTime);
		} else {
			transform.position = anchor;
		}
		
		// rotation update loop
		if (rotateTowards != null) {
			Vector3 relativeRotationVector = (rotateTowards.position + staticOffset_RotationAnchor) - anchor;
			transform.rotation = Quaternion.LookRotation(relativeRotationVector, Vector3.forward);
		}

		// scale update loop
		if (scaleTowards != null) {
			Canvas canvas = GetComponentInParent<Canvas>();
			Vector2 canvasSpaceTo = CanvasSpace(scaleTowards.position, canvas);
			Vector2 canvasSpaceAnchor = CanvasSpace(ZeroZAnchor, canvas);

			// scale based on canvas-space distance, but don't shrink past original size
			float canvasSpaceDistance = (canvasSpaceTo - canvasSpaceAnchor).magnitude;
			float scaledDelta = Mathf.Max((canvasSpaceDistance - sizeDeltaOffset), originalSizeDelta.y);
			GetComponent<RectTransform>().sizeDelta = new Vector2(originalSizeDelta.x, scaledDelta);
		}
	}

	public void AnchorTo(Transform _anchoredTransform) {
		anchoredTransform = _anchoredTransform;
	}

	public void AnchorRotationTowards(Transform _rotateTowards) {
		rotateTowards = _rotateTowards;
	}

	public void ScaleTowards(Transform _scaleTowards, float sizeDeltaOffset = 0f) {
		scaleTowards = _scaleTowards;
		this.sizeDeltaOffset = sizeDeltaOffset;
		originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
	}

	public void ShiftAnchorOffset(Vector3 direction, float magnitude, bool shiftRotationAnchor = false) {
		staticOffset += magnitude*direction;

		if (shiftRotationAnchor) {
			staticOffset_RotationAnchor += (1.5f*magnitude)*direction;
		}
	}

	private Vector2 CanvasSpace(Vector3 worldPosition, Canvas canvas) {
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// 0,0 for the canvas is at the center of the screen,
		// whereas WorldToViewPortPoint treats the lower left corner as 0,0.
		// Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
		return new Vector2(
			(viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x*0.5f),
			(viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y*0.5f)
		);
	}
}