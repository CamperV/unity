using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBobber : MonoBehaviour
{
	[HideInInspector] public Vector3 anchor; // this is in world-space
	private Transform anchoredTransform;

	[SerializeField] public float minViewportBound;
	[SerializeField] public float maxViewportBound;

	[SerializeField] private float freq;
	[SerializeField] private float amplitude;
	[SerializeField] private float phase;

	[Range(-1f, 1f)]
	[SerializeField] private float xDamping;

	void Start() {
		anchor = transform.position;
	}

	void Update() {
		if (anchoredTransform != null) {
			Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(anchoredTransform.position);
			cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
			Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
			
			anchor = new Vector3(transform.position.x, worldSpaceAnchor.y, transform.position.z);
		}

		Vector3 yComponent = amplitude * (Mathf.Sin( (freq*Time.time) + phase)) * Vector3.up;
		Vector3 xComponent = xDamping * amplitude * (Mathf.Cos( (freq*Time.time) + phase)) * Vector3.right;
		transform.position = Vector3.Lerp(transform.position, anchor + yComponent + xComponent, 10f*Time.deltaTime);
	}

	public void MoveAnchor(Vector3 inputWorldAnchor) {
		Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(inputWorldAnchor);
		cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
		Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
		
		anchor = new Vector3(transform.position.x, worldSpaceAnchor.y, transform.position.z);
	}

	public void TrackAnchor(Transform _anchoredTransform) {
		anchoredTransform = _anchoredTransform;
	}
}