using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_DamageText : MonoBehaviour, IEngagementPreviewer
{
	public Canvas targetCanvas;
	public MiniEngagementPreview miniEngagementPreviewPrefab;

	public void EnablePreview(Engagement potentialEngagement) {
		// place text above all targets
		foreach (Unit target in potentialEngagement.targets) {
			MiniEngagementPreview miniPreview_Defender = Instantiate(miniEngagementPreviewPrefab, transform);
			miniPreview_Defender.GetComponent<UIAnchor>().AnchorTo(target.transform);
			miniPreview_Defender.SetEngagementStats(potentialEngagement, true);
		}

		// and on top of the initiator as well
		MiniEngagementPreview miniPreview_Initiator = Instantiate(miniEngagementPreviewPrefab, transform);
		miniPreview_Initiator.GetComponent<UIAnchor>().AnchorTo(potentialEngagement.initiator.transform);
		miniPreview_Initiator.SetEngagementStats(potentialEngagement, false);

		Canvas.ForceUpdateCanvases();
	}

	public void DisablePreview(Engagement _) {
		foreach (Transform window in transform) {
			Destroy(window.gameObject);
		}
	}
}