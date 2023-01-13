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

		Damage fromEnemy_Health = potentialEngagement.counterAttacks.Select(ca => ca.damage).Aggregate((a, b) => a + b);
		Damage fromEnemy_Poise = potentialEngagement.counterAttacks.Select(ca => ca.poiseDamage).Aggregate((a, b) => a + b);
		//
		Damage fromPlayer_Health = potentialEngagement.attacks.Select(a => a.damage).Aggregate((a, b) => a + b);
		Damage fromPlayer_Poise = potentialEngagement.attacks.Select(a => a.poiseDamage).Aggregate((a, b) => a + b);

		// show the damage on the health bars proper
		aggUI.PreviewDamage(fromEnemy_Health, fromEnemy_Poise, isAggressor: true);
		defUI.PreviewDamage(fromPlayer_Health, fromPlayer_Poise);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.A.GetComponentInChildren<UnitUI>().RevertPreview();
		potentialEngagement.B.GetComponentInChildren<UnitUI>().RevertPreview();
	}
}