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
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();

		int comboDamage = 0;
		foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
			comboDamage += Mathf.Clamp(combo.damage - potentialEngagement.defense.damageReduction, 0, 99);
		}

		int minFromEnemy = enemyPreviewStats.finalDamageContext.Min*(potentialEngagement.defender.statSystem.MULTISTRIKE+1);
		int minFromPlayer = playerPreviewStats.finalDamageContext.Min*(potentialEngagement.aggressor.statSystem.MULTISTRIKE+1) + comboDamage;

		int maxFromEnemy = enemyPreviewStats.finalDamageContext.Max*(potentialEngagement.defender.statSystem.MULTISTRIKE+1);
		int maxFromPlayer = playerPreviewStats.finalDamageContext.Max*(potentialEngagement.aggressor.statSystem.MULTISTRIKE+1) + comboDamage;

		UnitUI aggUI = potentialEngagement.aggressor.GetComponentInChildren<UnitUI>();
		UnitUI defUI = potentialEngagement.defender.GetComponentInChildren<UnitUI>();

		// show the damage on the health bars proper
		aggUI.PreviewDamage(minFromEnemy, maxFromEnemy, isAggressor: true);
		defUI.PreviewDamage(minFromPlayer, maxFromPlayer);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.aggressor.GetComponentInChildren<UnitUI>().RevertPreview();
		potentialEngagement.defender.GetComponentInChildren<UnitUI>().RevertPreview();
	}
}