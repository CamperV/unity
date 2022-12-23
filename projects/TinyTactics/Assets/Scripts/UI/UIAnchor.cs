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
	private Vector2 originalSizeDelta;

	[SerializeField] public float minViewportBound = 0f;
	[SerializeField] public float maxViewportBound = 1f;
	
	[SerializeField] private Vector3 staticOffset;
	[SerializeField] private bool smooth;

	public Vector3 ZeroZAnchor => new Vector3(anchor.x, anchor.y, 0f);

	void Start() {
		anchor = transform.position;
	}

	void Update() {
		// position update loop
		if (anchoredTransform != null) {
			// Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(anchoredTransform.position);
			// // cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
			// Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
			// Debug.Assert(worldSpaceAnchor == anchoredTransform.position);
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
			Vector3 relativeVector = rotateTowards.position - anchor;
			transform.rotation = Quaternion.LookRotation(relativeVector, Vector3.forward);
		}

		// scale update loop
		if (scaleTowards != null) {
			RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
 
			//0,0 for the canvas is at the center of the screen,
			// whereas WorldToViewPortPoint treats the lower left corner as 0,0.
			// Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

 			Vector2 viewportSpaceTo = Camera.main.WorldToViewportPoint(scaleTowards.position);
			Vector2 screenSpaceTo = new Vector2(
				(viewportSpaceTo.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x*0.5f),
 				(viewportSpaceTo.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y*0.5f)
			);
			Debug.Log($"screen space to : {screenSpaceTo}");
		 	Vector2 viewportSpaceAnchor = Camera.main.WorldToViewportPoint(ZeroZAnchor);
			Vector2 screenSpaceAnchor = new Vector2(
				(viewportSpaceAnchor.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x*0.5f),
 				(viewportSpaceAnchor.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y*0.5f)
			);

			float canvasSpaceDistance = (screenSpaceTo - screenSpaceAnchor).magnitude;
			// float canvasSpaceDistance = (screenSpaceTo = screenSpaceAnchor).magnitude;
			GetComponent<RectTransform>().sizeDelta = new Vector2(originalSizeDelta.x, canvasSpaceDistance);
		}
	}

	public void AnchorTo(Transform _anchoredTransform) {
		anchoredTransform = _anchoredTransform;
	}

	public void AnchorRotationTowards(Transform _rotateTowards) {
		rotateTowards = _rotateTowards;
	}

	public void ScaleTowards(Transform _scaleTowards) {
		scaleTowards = _scaleTowards;
		originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
	}
}