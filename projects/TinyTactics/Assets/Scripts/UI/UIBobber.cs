using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBobber : MonoBehaviour
{
	[HideInInspector] public Vector3 anchor; // this is in world-space
	private Transform anchoredTransform;

	[SerializeField] public float minViewportBound = 0f;
	[SerializeField] public float maxViewportBound = 1f;

	[SerializeField] private float freq;
	[SerializeField] private float amplitude;
	[SerializeField] private float phase;
	
	[SerializeField] private Vector3 staticOffset;

	[Range(-1f, 1f)]
	[SerializeField] private float xDamping;

	public bool lockOntoX;
	public bool randomPhaseSalt;

	void Start() {
		anchor = transform.position;

		if (randomPhaseSalt) phase += Random.Range(0, 10);
	}

	void Update() {
		if (anchoredTransform != null) {
			Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(anchoredTransform.position);
			cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
			Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
			
			if (lockOntoX) {
				anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z);	
			} else {
				anchor = new Vector3(transform.position.x, worldSpaceAnchor.y, transform.position.z);
			}
		}

		Vector3 yComponent = amplitude * (Mathf.Sin( (freq*Time.time) + phase)) * Vector3.up;
		Vector3 xComponent = xDamping * amplitude * (Mathf.Cos( (freq*Time.time) + phase)) * Vector3.right;
		anchor = anchor + staticOffset;
		transform.position = Vector3.Lerp(transform.position, anchor + yComponent + xComponent, 10f*Time.deltaTime);
	}

	public void MoveAnchor(Vector3 inputWorldAnchor) {
		Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(inputWorldAnchor);
		cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
		Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
		
		if (lockOntoX) {
			anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z);	
		} else {
			anchor = new Vector3(transform.position.x, worldSpaceAnchor.y, transform.position.z);
		}
	}

	public void MoveAnchorOffset(Vector3 inputWorldAnchor, Vector3 offset) {
		Vector3 cameraSpaceAnchor = Camera.main.WorldToViewportPoint(inputWorldAnchor);
		cameraSpaceAnchor.y = Mathf.Clamp(cameraSpaceAnchor.y, minViewportBound, maxViewportBound);
		Vector3 worldSpaceAnchor = Camera.main.ViewportToWorldPoint(cameraSpaceAnchor);
		
		if (lockOntoX) {
			anchor = new Vector3(worldSpaceAnchor.x, worldSpaceAnchor.y, transform.position.z) + offset;	
		} else {
			anchor = new Vector3(transform.position.x, worldSpaceAnchor.y, transform.position.z) + offset;
		}
	}

	public void TrackAnchor(Transform _anchoredTransform) {
		anchoredTransform = _anchoredTransform;
	}
}