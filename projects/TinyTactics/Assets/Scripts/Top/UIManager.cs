using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	public static UIManager inst = null; // enforces singleton behavior

    [SerializeField] private Text currentTurnText;
	[SerializeField] private Text currentPhaseText;

	[SerializeField] private UnitDetailPanel unitDetailPanel;

	[SerializeField] private GameObject engagementPreviewContainer;
	[SerializeField] private EngagementPreviewPanel playerEngagementPreviewPanel;
	[SerializeField] private EngagementPreviewPanel enemyEngagementPreviewPanel;

	public CombatLog combatLog;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		engagementPreviewContainer.SetActive(false);
    }

	public void UpdateTurn(int newTurn) {
		currentTurnText.text = $"Turn {newTurn}";
	}

	public void UpdatePhase(Phase newPhase) {
		currentPhaseText.text = $"Current Phase: {newPhase.name}"; 
	}

	public void EnableUnitDetail(Unit unit) {
		unitDetailPanel.gameObject.SetActive(true);

	    unitDetailPanel.portraitImage.sprite = unit.spriteRenderer.sprite;
		unitDetailPanel.portraitImage.color = unit.spriteRenderer.color;
		// unitDetailPanel.nameText.SetText(unit.name);
		unitDetailPanel.weaponImage.sprite = unit.equippedWeapon.sprite;
		unitDetailPanel.weaponImage.color = unit.equippedWeapon.color;
		unitDetailPanel.weaponNameText.SetText($"{unit.equippedWeapon.name}");
		
		// attributes
		unitDetailPanel.hpValue.SetText($"{unit.unitStats._CURRENT_HP}/{unit.unitStats.VITALITY}");
		unitDetailPanel.drValue.SetText($"{unit.unitStats.DAMAGE_REDUCTION}");
		//
		unitDetailPanel.vitValue.SetText($"{unit.unitStats.VITALITY}");
		unitDetailPanel.strValue.SetText($"{unit.unitStats.STRENGTH}");
		unitDetailPanel.dexValue.SetText($"{unit.unitStats.DEXTERITY}");
		unitDetailPanel.refValue.SetText($"{unit.unitStats.REFLEX}");
		unitDetailPanel.movValue.SetText($"{unit.unitStats.MOVE}");

		// derived
		int atk = unit.unitStats.STRENGTH + unit.equippedWeapon.weaponStats.MIGHT;
		int hit = unit.unitStats.DEXTERITY + unit.equippedWeapon.weaponStats.ACCURACY;
		int avo = unit.unitStats.REFLEX*2 - Mathf.Max(0, (unit.equippedWeapon.weaponStats.WEIGHT - unit.unitStats.STRENGTH));
		unitDetailPanel.atkValue.SetText($"{atk}");
		unitDetailPanel.hitValue.SetText($"{hit}");
		unitDetailPanel.avoValue.SetText($"{avo}");

		// perks
		List<string> unitPerks = new List<string>();
		foreach (IMutatorComponent mc in unit.GetComponents<IMutatorComponent>()) {
			unitPerks.Add(mc.displayName);
		}
		string unitPerksText = string.Join("\n", unitPerks);
		unitDetailPanel.unitPerksValue.SetText(unitPerksText);
	}

	public void DisableUnitDetail() {
		unitDetailPanel.gameObject.SetActive(false);
	}

	public void EnableEngagementPreview(Engagement potentialEngagement, Transform anchoredTransform) {
		engagementPreviewContainer.SetActive(true);

		playerEngagementPreviewPanel.GetComponent<UIBobber>().TrackAnchor(anchoredTransform);
		enemyEngagementPreviewPanel.GetComponent<UIBobber>().TrackAnchor(anchoredTransform);

		// PLAYER-SIDE
		Engagement.Stats playerPreviewStats = potentialEngagement.SimulateAttack();
		playerEngagementPreviewPanel.portraitImage.sprite = potentialEngagement.aggressor.spriteRenderer.sprite;
		playerEngagementPreviewPanel.portraitImage.color = potentialEngagement.aggressor.spriteRenderer.color;
		playerEngagementPreviewPanel.weaponImage.sprite = potentialEngagement.aggressor.equippedWeapon.sprite;
		playerEngagementPreviewPanel.weaponImage.color = potentialEngagement.aggressor.equippedWeapon.color;
		//
		playerEngagementPreviewPanel.hpValue.SetText($"{potentialEngagement.aggressor.unitStats._CURRENT_HP}");
		playerEngagementPreviewPanel.dmgValue.SetText($"{playerPreviewStats.damage}");
		playerEngagementPreviewPanel.hitValue.SetText($"{playerPreviewStats.hitRate}%");
		playerEngagementPreviewPanel.critValue.SetText($"{playerPreviewStats.critRate}%");

		// list of perks that were relevant for this Attack & potentially, counterDefense
		List<string> playerUnitMutators = new List<string>(potentialEngagement.attack.mutators);
		if (potentialEngagement.counterDefense != null) {
			playerUnitMutators = playerUnitMutators.Concat(potentialEngagement.counterDefense.Value.mutators).Distinct().ToList();
		}
		string playerUnitMutatorsText = string.Join("\n", playerUnitMutators);
		playerEngagementPreviewPanel.mutatorsValue.SetText(playerUnitMutatorsText);


		// ENEMY-SIDE
		// only update this if you CAN counter-attack
		Engagement.Stats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
		enemyEngagementPreviewPanel.portraitImage.sprite = potentialEngagement.defender.spriteRenderer.sprite;
		enemyEngagementPreviewPanel.portraitImage.color = potentialEngagement.defender.spriteRenderer.color;
		enemyEngagementPreviewPanel.weaponImage.sprite = potentialEngagement.defender.equippedWeapon.sprite;
		enemyEngagementPreviewPanel.weaponImage.color = potentialEngagement.defender.equippedWeapon.color;
		//
		enemyEngagementPreviewPanel.hpValue.SetText($"{potentialEngagement.defender.unitStats._CURRENT_HP}");

		if (enemyPreviewStats.Empty) {
			enemyEngagementPreviewPanel.dmgValue.SetText($"--");
			enemyEngagementPreviewPanel.hitValue.SetText($"--%");
			enemyEngagementPreviewPanel.critValue.SetText($"--%");
		} else {
			enemyEngagementPreviewPanel.hpValue.SetText($"{potentialEngagement.defender.unitStats._CURRENT_HP}");
			enemyEngagementPreviewPanel.dmgValue.SetText($"{enemyPreviewStats.damage}");
			enemyEngagementPreviewPanel.hitValue.SetText($"{enemyPreviewStats.hitRate}%");
			enemyEngagementPreviewPanel.critValue.SetText($"{enemyPreviewStats.critRate}%");	
		}

		
		// list of perks that were relevant for this Defense & potentially, counterAttack
		List<string> enemyUnitMutators = new List<string>(potentialEngagement.defense.mutators);
		if (potentialEngagement.counterAttack != null) {
			enemyUnitMutators = enemyUnitMutators.Concat(potentialEngagement.counterAttack.Value.mutators).Distinct().ToList();
		}
		string enemyUnitMutatorsText = string.Join("\n", enemyUnitMutators);
		enemyEngagementPreviewPanel.mutatorsValue.SetText(enemyUnitMutatorsText);
	}

	public void DisableEngagementPreview() {
		engagementPreviewContainer.SetActive(false);

		playerEngagementPreviewPanel.GetComponent<UIBobber>().TrackAnchor(null);
		enemyEngagementPreviewPanel.GetComponent<UIBobber>().TrackAnchor(null);
	}
}