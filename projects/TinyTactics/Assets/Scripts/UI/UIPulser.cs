using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UIPulser : MonoBehaviour
{
	[SerializeField] private Vector3 finalScale;
	[SerializeField] private float pulsePeriod;
	[SerializeField] private float pulseLength;

	[SerializeField] private Image targetImage;

	void Awake() {
		if (targetImage == null) targetImage = GetComponent<Image>();
	}

	void OnEnable() {
		StartCoroutine( BetweenPulses() );
	}

	void OnDisable() {
		StopAllCoroutines();
		
		foreach (Transform childTransform in transform) {
			Destroy(childTransform.gameObject);
		}
	}

	private IEnumerator BetweenPulses() {
		while(true) {
			yield return Pulse();
			yield return new WaitForSeconds(pulsePeriod);
		}
	}

	private IEnumerator Pulse() {
		// spawn a copy of the attached image
		// increase it's scale over the period
		// while also decreasing its alpha
		GameObject pulseClone = new GameObject();
		pulseClone.name = $"{targetImage.gameObject.name}_PulseClone";
		pulseClone.AddComponent<RectTransform>();
		pulseClone.transform.SetParent(targetImage.gameObject.transform);
		
		pulseClone.GetComponent<RectTransform>().sizeDelta = targetImage.gameObject.GetComponent<RectTransform>().sizeDelta;
		pulseClone.GetComponent<RectTransform>().localScale = Vector3.one;
		pulseClone.GetComponent<RectTransform>().localPosition = Vector3.zero;
		//
		CanvasGroup canvasGroup = pulseClone.AddComponent<CanvasGroup>();
		Image im = pulseClone.AddComponent<Image>();
		im.sprite = targetImage.sprite;

		float timeRatio = 0.0f;
		Vector3 initialScale = pulseClone.transform.localScale;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / pulseLength);

			// smooth transparency & scale increase
			pulseClone.transform.localScale = Vector3.Lerp(initialScale, finalScale, timeRatio);
			canvasGroup.alpha = (1.0f - timeRatio);

			yield return null;
		}

		Destroy(pulseClone);
	}
}