using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBobber : MonoBehaviour
{
	[HideInInspector] public Vector3 anchor;

	[SerializeField] private float freq;
	[SerializeField] private float amplitude;
	[SerializeField] private float phase;

	[Range(-1f, 1f)]
	[SerializeField] private float xDamping;

	void Start() {
		anchor = transform.position;
	}

	void Update() {
		Vector3 yComponent = amplitude * (Mathf.Sin( (freq*Time.time) + phase)) * Vector3.up;
		Vector3 xComponent = xDamping * amplitude * (Mathf.Cos( (freq*Time.time) + phase)) * Vector3.right;
		transform.position = Vector3.Lerp(transform.position, anchor + yComponent + xComponent, 10f*Time.deltaTime);
	}

	public void MoveAnchor(float yAnchor) {
		anchor = new Vector3(transform.position.x, yAnchor, transform.position.z);
	}
}