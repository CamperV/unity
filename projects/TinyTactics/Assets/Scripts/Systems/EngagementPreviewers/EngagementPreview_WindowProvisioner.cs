using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_WindowProvisioner : MonoBehaviour, IEngagementPreviewer
{
	public Canvas targetCanvas;
	public MiniEngagementPreview miniEngagementPreviewPrefab;

	public void EnablePreview(Engagement potentialEngagement) {
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();

		MiniEngagementPreview miniPreview_Aggressor = Instantiate(miniEngagementPreviewPrefab, transform);
		MiniEngagementPreview miniPreview_Defender  = Instantiate(miniEngagementPreviewPrefab, transform);
		
		// then, position yourself above the unit				
		miniPreview_Aggressor.GetComponent<UIAnchor>().AnchorTo(potentialEngagement.aggressor.transform);
		miniPreview_Defender.GetComponent<UIAnchor>().AnchorTo(potentialEngagement.defender.transform);

		// set appropriate values, and ensure the previews are destroyed when the EngagementPreview proper is disabled
		miniPreview_Aggressor.SetEngagementStats(potentialEngagement, playerPreviewStats, true);
		miniPreview_Defender.SetEngagementStats(potentialEngagement, enemyPreviewStats, false);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		foreach (Transform window in transform) {
			Destroy(window.gameObject);
		}
	}
}