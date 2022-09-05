using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnchoredBobberProvisioner : MonoBehaviour
{
	[SerializeField] private float freq = 1f;
	[SerializeField] private Vector3 amplitude;
	[SerializeField] private float phaseOffsetBetweenChildren = 0.5f;

	private LayoutGroup layoutGroup;
	private int currentNumberOfChildren;

	void Awake() {
		layoutGroup = GetComponentInChildren<LayoutGroup>();
	}

	void OnEnable() {
		currentNumberOfChildren = 0;
	}

	// we have to check every frame if new children are added
	void Update() {
		UIAnchoredBobber[] bobberChildren = layoutGroup.GetComponentsInChildren<UIAnchoredBobber>();

		if (bobberChildren.Length != currentNumberOfChildren) {
			AssignData(bobberChildren);
		}

		currentNumberOfChildren = bobberChildren.Length;
	}

	private void AssignData(IEnumerable<UIAnchoredBobber> children) {
		float phaseOffset = 0f;

		foreach (UIAnchoredBobber child in children) {
			child.SetData(freq, amplitude, phaseOffset);
			phaseOffset += phaseOffsetBetweenChildren;
		}
	}
}
