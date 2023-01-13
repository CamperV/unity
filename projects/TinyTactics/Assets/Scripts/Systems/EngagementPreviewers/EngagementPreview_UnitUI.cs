using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_UnitUI : MonoBehaviour, IEngagementPreviewer
{
	// display the damage dealt on various UnitUI fields,
	// such as health bar or poise bar or defense segments
	public void EnablePreview(Engagement potentialEngagement) {
		UnitUI aggUI = potentialEngagement.A.GetComponentInChildren<UnitUI>();
		UnitUI defUI = potentialEngagement.B.GetComponentInChildren<UnitUI>();

		Damage fromEnemy_Health = potentialEngagement.TotalDamage(counter: true);
		Damage fromEnemy_Poise = potentialEngagement.TotalPoiseDamage(counter: true);
		//
		Damage fromPlayer_Health = potentialEngagement.TotalDamage();
		Damage fromPlayer_Poise = potentialEngagement.TotalPoiseDamage();

		// show the damage on the health bars proper
		aggUI.PreviewDamage(fromEnemy_Health, fromEnemy_Poise, isAggressor: true);
		defUI.PreviewDamage(fromPlayer_Health, fromPlayer_Poise);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.A.GetComponentInChildren<UnitUI>().RevertPreview();
		potentialEngagement.B.GetComponentInChildren<UnitUI>().RevertPreview();
	}
}