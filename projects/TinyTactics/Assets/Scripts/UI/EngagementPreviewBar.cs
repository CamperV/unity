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
	[SerializeField] private UIDamageProjector projectedDamage_Player;
	[SerializeField] private UIMultistrikeDisplay multistrikeDisplay_Player;

	[SerializeField] private GameObject mutatorsContainer_Player;
	[SerializeField] private TextMeshProUGUI mutatorsList_Player;
	[SerializeField] private Image portrait_Player;

	[Header("Enemy Side")]
    [SerializeField] private SegmentedHealthBarUI healthBar_Enemy;
	[SerializeField] private UIDamageProjector projectedDamage_Enemy;
	[SerializeField] private UIMultistrikeDisplay multistrikeDisplay_Enemy;

	[SerializeField] private GameObject mutatorsContainer_Enemy;
	[SerializeField] private TextMeshProUGUI mutatorsList_Enemy;
	[SerializeField] private Image portrait_Enemy;

	void OnEnable() {
		foreach (var lc in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.MarkLayoutForRebuild(lc.GetComponent<RectTransform>());
		}
	}

	void OnDisable() {
		healthBar_Player.Clear();
		healthBar_Enemy.Clear();
		
		mutatorsContainer_Player.SetActive(false);
		mutatorsContainer_Enemy.SetActive(false);
	}

	public void SetEngagementStats(Engagement potentialEngagement) {
		// player-side first
		healthBar_Player.AttachTo(potentialEngagement.aggressor);
		portrait_Player.sprite = potentialEngagement.aggressor.portraitSprite;

		// get the simulated damage and display it (w/ mutlistrike)
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		projectedDamage_Player.DisplayDamageProjection(playerPreviewStats, potentialEngagement.aggressor.unitStats._MULTISTRIKE);
		// multistrikeDisplay_Player.DisplayMultistrike(potentialEngagement.aggressor.unitStats._MULTISTRIKE);		

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

		//
		// then enemy-side
		//
		healthBar_Enemy.AttachTo(potentialEngagement.defender);
		portrait_Enemy.sprite = potentialEngagement.defender.portraitSprite;

		// how much damage can we do to the Enemy? (need to do this AFTER the health bar is attached to the enemy)
		healthBar_Enemy.PreviewDamage(playerPreviewStats.finalDamageContext.Max * (potentialEngagement.aggressor.unitStats._MULTISTRIKE+1));

		// get the simulated damage and display it (w/ mutlistrike)	
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
		projectedDamage_Enemy.DisplayDamageProjection(enemyPreviewStats, potentialEngagement.defender.unitStats._MULTISTRIKE);
		// multistrikeDisplay_Enemy.DisplayMultistrike(potentialEngagement.defender.unitStats._MULTISTRIKE);		

		if (!enemyPreviewStats.Empty) {
			// how much damage can we do to the Player?
			healthBar_Player.PreviewDamage(enemyPreviewStats.finalDamageContext.Max * (potentialEngagement.defender.unitStats._MULTISTRIKE+1));
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

		// Finally:
		// advantageIndicator_Player.SetActive(playerPreviewStats.hasAdvantage);
		// disadvantageIndicator_Player.SetActive(playerPreviewStats.hasDisadvantage);
		// advantageIndicator_Enemy.SetActive(enemyPreviewStats.hasAdvantage);
		// disadvantageIndicator_Enemy.SetActive(enemyPreviewStats.hasDisadvantage);
	}
}