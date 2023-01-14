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
		// for each target, need to preview all damage done from the player (aka in attacksList)
		foreach (Unit target in potentialEngagement.targets) {
			UnitUI defUI = target.GetComponentInChildren<UnitUI>();
			Damage fromPlayer_Health = potentialEngagement.TotalDamageTargeting(target);
			Damage fromPlayer_Poise = potentialEngagement.TotalPoiseDamageTargeting(target);
			defUI.PreviewDamage(fromPlayer_Health, fromPlayer_Poise);
		}

		// and also all incoming damage to the player
		// this only really needs to come from counterList
		// don't split it out
		UnitUI aggUI = potentialEngagement.initiator.GetComponentInChildren<UnitUI>();
		Damage fromEnemy_Health = potentialEngagement.TotalDamage(counter: true);
		Damage fromEnemy_Poise = potentialEngagement.TotalPoiseDamage(counter: true);
		aggUI.PreviewDamage(fromEnemy_Health, fromEnemy_Poise, isAggressor: true);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.initiator.GetComponentInChildren<UnitUI>().RevertPreview();
		foreach (Unit target in potentialEngagement.targets) {
			target.GetComponentInChildren<UnitUI>().RevertPreview();
		}
	}
}