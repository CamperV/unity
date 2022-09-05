using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchoredBobber : MonoBehaviour
{
	private Vector3 anchoredPosition;
	private RectTransform rectTransform;

	[SerializeField] private float freq = 1f;
	[SerializeField] private Vector3 amplitude;
	[SerializeField] private float phase = 0f;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}

	void OnEnable() {
		anchoredPosition = rectTransform.anchoredPosition;
	}

	void Update() {
		Vector3 yComponent = amplitude.y * (Mathf.Sin( (freq*Time.time) + phase)) * Vector3.up;
		Vector3 xComponent = amplitude.x * (Mathf.Cos( (freq*Time.time) + phase)) * Vector3.right;
		Vector3 destination = anchoredPosition + yComponent + xComponent;

		rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, destination, 10f*Time.deltaTime);
	}

	public void SetData(float f, Vector3 a, float p) {
		freq = f;
		amplitude = a;
		phase = p;
	}
}