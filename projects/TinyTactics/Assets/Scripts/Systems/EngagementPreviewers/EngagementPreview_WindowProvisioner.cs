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
	}

	public void DisablePreview(Engagement potentialEngagement) {
		previewWindow.gameObject.SetActive(false);
	}
}