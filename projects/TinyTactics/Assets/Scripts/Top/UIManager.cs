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
	[SerializeField] private BasicAttackInspection unitInspector;
	[SerializeField] private UnitCommandPanel unitCommandPanel;

	// deprecated above
	[SerializeField] private EngagementPreviewBar engagementPreviewBar;

	[SerializeField] private EndgameStatsPanel victoryPanel;
	[SerializeField] private EndgameStatsPanel defeatPanel;

	[SerializeField] private GameObject menuButtons;

	public CombatLog combatLog;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}

		startBattleButtonContainer.SetActive(true);
		unitInspector.gameObject.SetActive(false);
		engagementPreviewBar.gameObject.SetActive(false);
    }

	public void UpdateTerrainEffectPanel(TerrainTile terrainAt) {
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
		unitInspector.gameObject.SetActive(true);
		unitInspector.SetUnitInfo(unit);
		
		unitInspector.GetComponent<UIBobber>().TrackAnchor(unit.transform);
		unitInspector.GetComponent<UIBobber>().MoveAnchorOffset(unit.transform.position, 1.0f*Vector3.up);
	}

	public void DisableUnitDetail() {
		// menuButtons.SetActive(true);
		//
		unitInspector.gameObject.SetActive(false);
	}

	public void EnableEngagementPreview(Engagement potentialEngagement, Transform _) {
		engagementPreviewBar.gameObject.SetActive(true);
		engagementPreviewBar.GetComponent<UIAnchoredSlider>().SetActive(true, teleportInactiveFirst: true);
		engagementPreviewBar.SetEngagementStats(potentialEngagement);
	}

	public void DisableEngagementPreview() {
		engagementPreviewBar.gameObject.SetActive(false);
		engagementPreviewBar.GetComponent<UIAnchoredSlider>().SetActive(false);
	}

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
		} else {
			unitCommandPanel.GetComponent<UIAnchoredSlider>().SetActive(true, teleportInactiveFirst: true);
			unitCommandPanel.SetUnitInfo(unit);
		}
	}

	public void CleanUpUnitCommandPanel(PlayerUnit droppedUnit) {
		unitCommandPanel.ClearUnitInfo(droppedUnit);
	}
}