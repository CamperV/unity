using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

[RequireComponent(typeof(UIAnimator))]
public class UIFadeInOut : MonoBehaviour
{
	public bool fadeInOnEnable;
	public bool fadeOutOnDisable;

	public float fadeIn = 1.0f;
	public float fadeOut = 1.0f;

	private UIAnimator uiAnimator;

	void Awake() {
		uiAnimator = GetComponent<UIAnimator>();
	}

	void OnEnable() {
		if (fadeInOnEnable)
			uiAnimator.TriggerFadeUp(fadeIn);
	}

	void OnDisable() {
		if (fadeOutOnDisable)
			uiAnimator.TriggerFadeDown(fadeOut);
	}
}