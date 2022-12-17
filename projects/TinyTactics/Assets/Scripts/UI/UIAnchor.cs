using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchor : MonoBehaviour
{
	[HideInInspector] public Vector3 anchor; // this is in world-space
	private Transform anchoredTransform;

	[SerializeField] public float minViewportBound = 0f;
	[SerializeField] public float maxViewportBound = 1f;
	
	[SerializeField] private Vector3 staticOffset;
	[SerializeField] private bool smooth;

	void Start() {
		anchor = transform.position;
	}

	void Update() {
		if (anchoredTransform != null) {
			Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(anchoredTransform.position);
			// cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
			Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
			
			anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z);	
		}
		anchor = anchor + staticOffset;

		if (smooth) {
			transform.position = Vector3.Lerp(transform.position, anchor, 10f*Time.deltaTime);
		} else {
			transform.position = anchor;
		}
		
	}

	public void MoveAnchor(Vector3 inputWorldAnchor) {
		Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(inputWorldAnchor);
		// cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
		Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
		
		anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z);	
	}

	public void AnchorTo(Transform _anchoredTransform) {
		anchoredTransform = _anchoredTransform;
	}
}