using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_WindowProvisioner : MonoBehaviour, IEngagementPreviewer
{
	public EngagementPreviewWindow previewWindow;
	private UIAnimator windowAnimator;

	void Awake() {
		windowAnimator = previewWindow.GetComponent<UIAnimator>();
	}

	public void EnablePreview(Engagement potentialEngagement) {
		previewWindow.gameObject.SetActive(true);
		previewWindow.SetEngagementStats(potentialEngagement);

		// cancel any fade downs, start fade up
		windowAnimator.TriggerFadeUp(0.1f);
	}

	public void DisablePreview(Engagement _) {
		// cancel any fade ups, start fade down
		// after fade down, set active false
		windowAnimator.TriggerFadeDownDisable(0.1f);
	}
}