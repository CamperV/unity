using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

[RequireComponent(typeof(UIAnimator))]
public class UIFlasher : MonoBehaviour
{
	[SerializeField] private Color color_0;
	[SerializeField] private Color color_1;
	private UIAnimator animator;

	void Awake() {
		animator = GetComponent<UIAnimator>();
	}

	void OnEnable() {
		StartCoroutine( animator.Flash(color_0, color_1) );
	}

	void OnDisable() {
		StopAllCoroutines();
	}
}