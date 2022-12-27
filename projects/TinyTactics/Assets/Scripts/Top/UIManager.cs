using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	public static UIManager inst = null; // enforces singleton behavior

	[SerializeField] private GameObject startBattleButtonContainer;

	[SerializeField] private TerrainEffectPanel terrainEffectPanel;
	[SerializeField] private UnitCommandPanel unitCommandPanel;
	[SerializeField] private UnitInspector[] unitInspectors;

	[SerializeField] private EngagementPreviewBar engagementPreviewBar;
	// [SerializeField] private MiniEngagementPreview miniEngagementPreviewPrefab;

	[SerializeField] private EndgameStatsPanel victoryPanel;
	[SerializeField] private EndgameStatsPanel defeatPanel;

	[SerializeField] private GameObject menuButtons;

	// for binding UI, etc
    // public delegate void EngagementPreviewEvent();
	// public event EngagementPreviewEvent EnableEngagementPreviewEvent;
    // public event EngagementPreviewEvent DisableEngagementPreviewEvent;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}

		startBattleButtonContainer.SetActive(true);
		engagementPreviewBar?.gameObject.SetActive(false);

		foreach (UnitInspector unitInspector in unitInspectors) {
			unitInspector?.gameObject.SetActive(false);
		}
    }

	public void UpdateTerrainEffectPanel(GridPosition _, TerrainTile terrainAt) {
		terrainEffectPanel.tileValue.sprite = terrainAt.sprite;

		if (terrainAt.HasTerrainEffect) {
			terrainEffectPanel.effectValue.SetText($"{terrainAt.displayName}: {terrainAt.terrainEffect.shortDisplayName}");
		} else {
			terrainEffectPanel.effectValue.SetText($"{terrainAt.displayName}: --");	
		}
	}

	public void EnableUnitDetail(Unit unit) {
		// menuButtons.SetActive(false);
		//
		foreach (UnitInspector unitInspector in unitInspectors) {
			unitInspector?.gameObject.SetActive(true);
			unitInspector?.SetUnitInfo(unit);
		}
	}

	public void DisableUnitDetail() {
		// menuButtons.SetActive(true);
		//
		foreach (UnitInspector unitInspector in unitInspectors) {
			unitInspector?.gameObject.SetActive(false);
		}
	}

	// public void EnableEngagementPreview(Engagement potentialEngagement) {
	// 	engagementPreviewBar?.gameObject.SetActive(true);
	// 	engagementPreviewBar?.GetComponent<UIAnchoredSlider>().SetActive(true, teleportInactiveFirst: true);

	// 	EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
	// 	EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
	// 	engagementPreviewBar?.SetEngagementStats(potentialEngagement, playerPreviewStats, enemyPreviewStats);
	// }

	// public void DisableEngagementPreview() {
	// 	engagementPreviewBar?.gameObject.SetActive(false);
	// 	engagementPreviewBar?.GetComponent<UIAnchoredSlider>().SetActive(false);

	// 	// invoke and immediately clear invocation list
	// 	// this is to clear all the anon functions we put on this from the MiniPreviews
	// 	DisableEngagementPreviewEvent?.Invoke();
	// 	DisableEngagementPreviewEvent = null;
	// }

	public void CreateVictoryPanel(int enemiesDefeated, int survivingUnits, int turnsElapsed) {
		victoryPanel.gameObject.SetActive(true);
		victoryPanel.enemiesDefeatedValue.SetText($"{enemiesDefeated}");
		victoryPanel.survivingUnitsValue.SetText($"{survivingUnits}");
		victoryPanel.turnsElapsedValue.SetText($"{turnsElapsed}");
	}

	public void CreateDefeatPanel(int enemiesDefeated, int survivingUnits, int turnsElapsed) {
		defeatPanel.gameObject.SetActive(true);
		defeatPanel.enemiesDefeatedValue.SetText($"{enemiesDefeated}");
		defeatPanel.survivingUnitsValue.SetText($"{survivingUnits}");
		defeatPanel.turnsElapsedValue.SetText($"{turnsElapsed}");
	}

	public void EnableUnitCommandPanel(PlayerUnit unit) {
		if (unit == null) {
			unitCommandPanel.GetComponent<UIAnchoredSlider>().SetActive(false);
			unitCommandPanel.gameObject.SetActive(false);
		} else {
			unitCommandPanel.gameObject.SetActive(true);
			unitCommandPanel.GetComponent<UIAnchoredSlider>().SetActive(true, teleportInactiveFirst: true);
			unitCommandPanel.SetUnitInfo(unit);
		}
	}

	public void CleanUpUnitCommandPanel(PlayerUnit droppedUnit) {
		unitCommandPanel.ClearUnitInfo(droppedUnit);
	}
}