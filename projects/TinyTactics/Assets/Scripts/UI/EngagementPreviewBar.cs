using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreviewBar : MonoBehaviour
{
	[Header("Player Side")]
    [SerializeField] private SegmentedHealthBarUI healthBar_Player;
	[SerializeField] private TextMeshProUGUI projectedDamage_Player;
	[SerializeField] private GameObject multstrikeContainer_Player;
	[SerializeField] private TextMeshProUGUI multistrikeValue_Player;
	[SerializeField] private GameObject mutatorsContainer_Player;
	[SerializeField] private TextMeshProUGUI mutatorsList_Player;

	[Header("Enemy Side")]
    [SerializeField] private SegmentedHealthBarUI healthBar_Enemy;
	[SerializeField] private TextMeshProUGUI projectedDamage_Enemy;
	[SerializeField] private GameObject multstrikeContainer_Enemy;
	[SerializeField] private TextMeshProUGUI multistrikeValue_Enemy;
	[SerializeField] private GameObject mutatorsContainer_Enemy;
	[SerializeField] private TextMeshProUGUI mutatorsList_Enemy;

	public void SetEngagementStats(Engagement potentialEngagement) {
		// player-side first
		healthBar_Player.AttachTo(potentialEngagement.aggressor);

		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		projectedDamage_Player.SetText($"{playerPreviewStats.minDamage} - {playerPreviewStats.maxDamage}");

		if (potentialEngagement.aggressor.unitStats._MULTISTRIKE > 0) {
			multstrikeContainer_Player.SetActive(true);
			multistrikeValue_Player.SetText($"x{potentialEngagement.aggressor.unitStats._MULTISTRIKE + 1}");
		}

		// list of perks that were relevant for this Attack & potentially, counterDefense
		List<string> playerUnitMutators = new List<string>(potentialEngagement.attack.mutators);
		if (potentialEngagement.counterDefense != null) {
			playerUnitMutators = playerUnitMutators.Concat(potentialEngagement.counterDefense.Value.mutators).ToList();
		}
		playerUnitMutators = playerUnitMutators.Concat(playerPreviewStats.mutators).ToList();

		if (playerUnitMutators.Count > 0) {
			mutatorsContainer_Player.SetActive(true);
			string playerUnitMutatorsText = string.Join("\n", playerUnitMutators.Distinct().ToList());
			mutatorsList_Player.SetText(playerUnitMutatorsText);
		}



		// then enemy-side
		healthBar_Enemy.AttachTo(potentialEngagement.defender);

		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
		if (enemyPreviewStats.Empty) {
			projectedDamage_Enemy.SetText($"-");
			multstrikeContainer_Enemy.SetActive(false);
		} else {
			projectedDamage_Enemy.SetText($"{enemyPreviewStats.minDamage} - {enemyPreviewStats.maxDamage}");

			if (potentialEngagement.defender.unitStats._MULTISTRIKE > 0) {
				multstrikeContainer_Enemy.SetActive(true);
				multistrikeValue_Enemy.SetText($"x{potentialEngagement.defender.unitStats._MULTISTRIKE + 1}");
			}
		}

		// list of perks that were relevant for this Defense & potentially, counterAttack
		List<string> enemyUnitMutators = new List<string>(potentialEngagement.defense.mutators);
		if (potentialEngagement.counterAttack != null) {
			enemyUnitMutators = enemyUnitMutators.Concat(potentialEngagement.counterAttack.Value.mutators).ToList();
		}
		enemyUnitMutators = enemyUnitMutators.Concat(enemyPreviewStats.mutators).ToList();

		if (enemyUnitMutators.Count > 0) {
			mutatorsContainer_Enemy.SetActive(true);
			string enemyUnitMutatorsText = string.Join("\n", enemyUnitMutators.Distinct().ToList());
			mutatorsList_Enemy.SetText(enemyUnitMutatorsText);
		}
	}
}