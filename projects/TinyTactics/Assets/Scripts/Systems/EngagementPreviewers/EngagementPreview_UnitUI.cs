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
		int minFromEnemy = potentialEngagement.counterAttacks.Sum(ca => ca.damage.min);
		int maxFromEnemy = potentialEngagement.counterAttacks.Sum(ca => ca.damage.max);

		int minFromPlayer = potentialEngagement.attacks.Sum(a => a.damage.min);
		int maxFromPlayer = potentialEngagement.attacks.Sum(a => a.damage.max);

		UnitUI aggUI = potentialEngagement.A.GetComponentInChildren<UnitUI>();
		UnitUI defUI = potentialEngagement.B.GetComponentInChildren<UnitUI>();

		// show the damage on the health bars proper
		aggUI.PreviewDamage(minFromEnemy, maxFromEnemy, isAggressor: true);
		defUI.PreviewDamage(minFromPlayer, maxFromPlayer);
	}

	public void DisablePreview(Engagement potentialEngagement) {
		potentialEngagement.A.GetComponentInChildren<UnitUI>().RevertPreview();
		potentialEngagement.B.GetComponentInChildren<UnitUI>().RevertPreview();
	}
}