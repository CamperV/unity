using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damagePreview;

	public void SetEngagementStats(Engagement potentialEngagement, EngagementStats previewStats, bool isAggressor) {
		damagePreview.SetText("");
		int numStrikes = (isAggressor) ? potentialEngagement.aggressor.statSystem.MULTISTRIKE+1 : potentialEngagement.aggressor.statSystem.MULTISTRIKE+1;

		// set damage value
		if (!previewStats.Empty) {
			int min = previewStats.finalDamageContext.Min * numStrikes;
			int max = previewStats.finalDamageContext.Max * numStrikes;

			string damage = $"{min}";
			if (min != max) {
				damage += $" - {max}";
			}

			// aka can you receive these combo attacks
			if (isAggressor == false) {
				foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
					damage += $"<size=20> \n+{combo.damage}</size>";
				}
			}

			damagePreview.SetText($"<bounce>{damage}</bounce>");
		}
	}
}