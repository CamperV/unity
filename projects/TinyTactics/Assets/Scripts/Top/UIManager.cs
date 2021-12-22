using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	public static UIManager inst = null; // enforces singleton behavior

    [SerializeField] private Text currentTurnText;
	[SerializeField] private Text currentPhaseText;

	[SerializeField] private GameObject engagementPreviewContainer;
	[SerializeField] private EngagementPreviewPanel playerEngagementPreviewPanel;
	[SerializeField] private EngagementPreviewPanel enemyEngagementPreviewPanel;
	
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

	public void EnableEngagementPreview(Engagement potentialEngagement) {
		engagementPreviewContainer.SetActive(true);

		// PLAYER-SIDE
		Engagement.Stats playerPreviewStats = potentialEngagement.SimulateAttack();
		playerEngagementPreviewPanel.portraitImage.sprite = potentialEngagement.aggressor.spriteRenderer.sprite;
		// playerEngagementPreviewPanel.weaponImage.sprite = ;
		playerEngagementPreviewPanel.hpValue.SetText($"{potentialEngagement.aggressor.unitStats._CURRENT_HP}");
		playerEngagementPreviewPanel.dmgValue.SetText($"{playerPreviewStats.damage}");
		playerEngagementPreviewPanel.hitValue.SetText($"{playerPreviewStats.hitRate}%");
		playerEngagementPreviewPanel.critValue.SetText($"{playerPreviewStats.critRate}%");

		// ENEMY-SIDE
		// only update this if you CAN counter-attack
		Engagement.Stats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
		if (!enemyPreviewStats.Empty) {
			enemyEngagementPreviewPanel.portraitImage.sprite = potentialEngagement.defender.spriteRenderer.sprite;
			// enemyEngagementPreviewPanel.weaponImage.sprite = ;
			enemyEngagementPreviewPanel.hpValue.SetText($"{potentialEngagement.defender.unitStats._CURRENT_HP}");
			enemyEngagementPreviewPanel.dmgValue.SetText($"{enemyPreviewStats.damage}");
			enemyEngagementPreviewPanel.hitValue.SetText($"{enemyPreviewStats.hitRate}%");
			enemyEngagementPreviewPanel.critValue.SetText($"{enemyPreviewStats.critRate}%");
		}
	}

	public void DisableEngagementPreview() {
		engagementPreviewContainer.SetActive(false);
	}
}