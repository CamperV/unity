using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damagePreview;

	public void SetEngagementStats(Engagement potentialEngagement, bool fromAggressor) {
		damagePreview.SetText("");

		List<Attack> attacks = (fromAggressor) ? potentialEngagement.attacks : potentialEngagement.counterAttacks;
		string damage = string.Join("\n", attacks.Select(a => a.damage.ToString()));

		damagePreview.SetText($"<bounce>{damage}</bounce>");
	}
}