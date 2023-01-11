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
		MiniEngagementPreview miniPreview_Aggressor = Instantiate(miniEngagementPreviewPrefab, transform);
		MiniEngagementPreview miniPreview_Defender  = Instantiate(miniEngagementPreviewPrefab, transform);
		
		// then, position yourself above the unit				
		miniPreview_Aggressor.GetComponent<UIAnchor>().AnchorTo(potentialEngagement.B.transform);
		miniPreview_Defender.GetComponent<UIAnchor>().AnchorTo(potentialEngagement.A.transform);

		// set appropriate values, and ensure the previews are destroyed when the EngagementPreview proper is disabled
		miniPreview_Aggressor.SetEngagementStats(potentialEngagement, true);
		miniPreview_Defender.SetEngagementStats(potentialEngagement, false);

		Canvas.ForceUpdateCanvases();
	}

	public void DisablePreview(Engagement _) {
		foreach (Transform window in transform) {
			Destroy(window.gameObject);
		}
	}
}