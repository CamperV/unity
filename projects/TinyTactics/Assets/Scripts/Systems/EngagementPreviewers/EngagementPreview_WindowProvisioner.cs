using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_WindowProvisioner : MonoBehaviour, IEngagementPreviewer
{
	public Canvas targetCanvas;
	public EngagementPreviewWindow previewWindow;

	public void EnablePreview(Engagement potentialEngagement) {
		previewWindow.gameObject.SetActive(true);
		previewWindow.SetEngagementStats(potentialEngagement);

		// cancel any fade downs, start fade up
		previewWindow.GetComponent<UIAnimator>().TriggerFadeUp(0.1f);
	}

	public void DisablePreview(Engagement _) {
		// cancel any fade ups, start fade down
		// after fade down, set active false
		previewWindow.GetComponent<UIAnimator>().TriggerFadeDownDisable(0.1f);
	}
}