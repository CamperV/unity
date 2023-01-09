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

		int finalProjectedDamage_fromEnemy = enemyPreviewStats.finalDamageContext.Max*(potentialEngagement.defender.statSystem.MULTISTRIKE+1);
		int finalProjectedDamage_fromPlayer = playerPreviewStats.finalDamageContext.Max*(potentialEngagement.aggressor.statSystem.MULTISTRIKE+1);
		foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
			finalProjectedDamage_fromPlayer += Mathf.Clamp(combo.damage - potentialEngagement.defense.damageReduction, 0, 99);
		}

		UnitUI aggUI = potentialEngagement.aggressor.GetComponentInChildren<UnitUI>();
		UnitUI defUI = potentialEngagement.defender.GetComponentInChildren<UnitUI>();

		// show the damage on the health bars proper
		aggUI.PreviewDamage(finalProjectedDamage_fromEnemy, isAggressor: true);
		defUI.PreviewDamage(finalProjectedDamage_fromPlayer);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.aggressor.GetComponentInChildren<UnitUI>().RevertPreview();
		potentialEngagement.defender.GetComponentInChildren<UnitUI>().RevertPreview();
	}
}