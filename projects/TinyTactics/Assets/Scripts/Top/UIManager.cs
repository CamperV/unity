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

	[SerializeField] private EndgameStatsPanel victoryPanel;
	[SerializeField] private EndgameStatsPanel defeatPanel;

	[SerializeField] private GameObject menuButtons;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}

		startBattleButtonContainer.SetActive(true);
    }

	public void UpdateTerrainEffectPanel(GridPosition _, TerrainTile terrainAt) {
		terrainEffectPanel.tileValue.sprite = terrainAt.sprite;

		if (terrainAt.HasTerrainEffect) {
			terrainEffectPanel.effectValue.SetText($"{terrainAt.displayName}: {terrainAt.terrainEffect.mutatorDisplayData.name}");
		} else {
			terrainEffectPanel.effectValue.SetText($"{terrainAt.displayName}: --");	
		}
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
}