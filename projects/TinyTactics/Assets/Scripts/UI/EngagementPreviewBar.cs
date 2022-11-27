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
	[SerializeField] private UIMutatorDisplay mutatorDisplay_Player;
	[SerializeField] private Image portrait_Player;
	//
	[SerializeField] private UIComboAttackDisplay comboAttackDisplay;

	[Header("Enemy Side")]
    [SerializeField] private SegmentedHealthBarUI healthBar_Enemy;
	[SerializeField] private UIDamageProjector projectedDamage_Enemy;
	[SerializeField] private UIMutatorDisplay mutatorDisplay_Enemy;
	[SerializeField] private Image portrait_Enemy;

	void OnEnable() {
		foreach (var layoutGroup in GetComponentsInChildren<LayoutGroup>()) {
        	LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    	}
	}

	void OnDisable() {
		healthBar_Player.Clear();
		healthBar_Player.Detach();
		healthBar_Enemy.Clear();
		healthBar_Enemy.Detach();
	}

	public void SetEngagementStats(Engagement potentialEngagement, EngagementStats playerPreviewStats, EngagementStats enemyPreviewStats) {
		// player-side first
		healthBar_Player.AttachTo(potentialEngagement.aggressor);
		portrait_Player.sprite = potentialEngagement.aggressor.portraitSprite;

		// get the simulated damage and display it (w/ mutlistrike)
		projectedDamage_Player.DisplayDamageProjection(potentialEngagement, playerPreviewStats, potentialEngagement.aggressor.unitStats._MULTISTRIKE);

		List<string> playerUnitMutators = new List<string>(potentialEngagement.attack.mutators);
		if (potentialEngagement.counterDefense != null) {
			playerUnitMutators = playerUnitMutators.Concat(potentialEngagement.counterDefense.Value.mutators).ToList();
		}
		playerUnitMutators = playerUnitMutators.Concat(playerPreviewStats.mutators).ToList();
		mutatorDisplay_Player.DisplayMutators(playerUnitMutators);

		//
		// then enemy-side
		//
		healthBar_Enemy.AttachTo(potentialEngagement.defender);
		portrait_Enemy.sprite = potentialEngagement.defender.portraitSprite;

		// how much damage can we do to the Enemy? (need to do this AFTER the health bar is attached to the enemy)
		int finalProjectedDamage_fromPlayer = playerPreviewStats.finalDamageContext.Max * (potentialEngagement.aggressor.unitStats._MULTISTRIKE+1);
		foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
			finalProjectedDamage_fromPlayer += Mathf.Clamp(combo.damage - potentialEngagement.defense.damageReduction, 0, 99);
		}
		healthBar_Enemy.PreviewDamage(finalProjectedDamage_fromPlayer);

		// get the simulated damage and display it (w/ mutlistrike)
		projectedDamage_Enemy.DisplayDamageProjection(enemyPreviewStats, potentialEngagement.defender.unitStats._MULTISTRIKE);

		// list of perks that were relevant for this Defense & potentially, counterAttack
		List<string> enemyUnitMutators = new List<string>(potentialEngagement.defense.mutators);
		if (potentialEngagement.counterAttack != null) {
			enemyUnitMutators = enemyUnitMutators.Concat(potentialEngagement.counterAttack.Value.mutators).ToList();
		}
		enemyUnitMutators = enemyUnitMutators.Concat(enemyPreviewStats.mutators).ToList();
		mutatorDisplay_Enemy.DisplayMutators(enemyUnitMutators);	

		// how much damage can we do to the Player?
		if (!enemyPreviewStats.Empty) {
			healthBar_Player.PreviewDamage(enemyPreviewStats.finalDamageContext.Max * (potentialEngagement.defender.unitStats._MULTISTRIKE+1));
		}

		// Finally:
		// advantageIndicator_Player.SetActive(playerPreviewStats.hasAdvantage);
		// disadvantageIndicator_Player.SetActive(playerPreviewStats.hasDisadvantage);
		// advantageIndicator_Enemy.SetActive(enemyPreviewStats.hasAdvantage);
		// disadvantageIndicator_Enemy.SetActive(enemyPreviewStats.hasDisadvantage);

		// SUPER Finally: include any combo-able allies and their attacks
		comboAttackDisplay.DisplayComboAttacks(potentialEngagement);
	}
}