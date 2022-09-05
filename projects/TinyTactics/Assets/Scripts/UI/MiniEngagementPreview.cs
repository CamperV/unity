using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview : MonoBehaviour
{
    [SerializeField] private TextMeshPro damagePreview;

	public void SetEngagementStats(EngagementStats previewStats) {
		Clear();

		if (!previewStats.Empty) {
			int min = previewStats.finalDamageContext.Min;
			int max = previewStats.finalDamageContext.Max;

			string damage = $"{min}";
			if (min != max) {
				damage += $" - {max}";
			}
			damagePreview.SetText($"{damage}");
		}
	}

	public void SetEngagementStats(EngagementStats previewStats, int numStrikes) {
		Clear();

		if (!previewStats.Empty) {
			int min = previewStats.finalDamageContext.Min * numStrikes;
			int max = previewStats.finalDamageContext.Max * numStrikes;

			string damage = $"{min}";
			if (min != max) {
				damage += $" - {max}";
			}
			damagePreview.SetText($"<bounce>{damage}</bounce>");
		}
	}

	private void Clear() {
		damagePreview.SetText("");
	}
}