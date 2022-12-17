using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview_Sprite : MonoBehaviour
{
    [SerializeField] private TextMeshPro damagePreview;

	public static void DrawPreviewArrow(Vector3 pos_0, Vector3 pos_1, Color color) {
		Debug.DrawLine(pos_0, pos_1, color: color, 5, false);
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