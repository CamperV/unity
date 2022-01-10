using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhaseAnnouncement : MonoBehaviour
{
    public TextMeshProUGUI announcementValue;
    [HideInInspector] private AudioSource audioSource;
    [HideInInspector] private CanvasGroup canvasGroup;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
		canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable() {
        canvasGroup.alpha = 1f;
        audioSource.PlayOneShot(audioSource.clip, 1f);
    }

    public IEnumerator FadeDown(float fadeTime) {
		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fadeTime);

			canvasGroup.alpha = (1.0f - timeRatio);
			yield return null;
		}
    }
}
