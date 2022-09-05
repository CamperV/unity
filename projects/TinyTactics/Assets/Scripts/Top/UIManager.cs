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

	[SerializeField] private EngagementPreviewBar engagementPreviewBar;
	[SerializeField] private MiniEngagementPreview miniEngagementPreviewPrefab;

	[SerializeField] private EndgameStatsPanel victoryPanel;
	[SerializeField] private EndgameStatsPanel defeatPanel;

	[SerializeField] private GameObject menuButtons;

	public CombatLog combatLog;

	// for binding UI, etc
    public delegate void EngagementPreviewEvent();
	public event EngagementPreviewEvent EnableEngagementPreviewEvent;
    public event EngagementPreviewEvent DisableEngagementPreviewEvent;
	
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

	public void EnableEngagementPreview(Engagement potentialEngagement) {
		engagementPreviewBar.gameObject.SetActive(true);
		engagementPreviewBar.GetComponent<UIAnchoredSlider>().SetActive(true, teleportInactiveFirst: true);

		
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();
		engagementPreviewBar.SetEngagementStats(potentialEngagement, playerPreviewStats, enemyPreviewStats);

		// also, create a little in-situ display
		// cross'd PreviewStats because they are displaying the damage they might *receive*
		MiniEngagementPreview miniPreview_Aggressor = Instantiate(miniEngagementPreviewPrefab, potentialEngagement.aggressor.transform);
		MiniEngagementPreview miniPreview_Defender  = Instantiate(miniEngagementPreviewPrefab, potentialEngagement.defender.transform);

		// set appropriate values, and ensure the previews are destroyed when the EngagementPreview proper is disabled
		miniPreview_Aggressor.SetEngagementStats(enemyPreviewStats, potentialEngagement.defender.unitStats._MULTISTRIKE+1);
		miniPreview_Defender.SetEngagementStats(playerPreviewStats, potentialEngagement.aggressor.unitStats._MULTISTRIKE+1);
		DisableEngagementPreviewEvent += () => Destroy(miniPreview_Aggressor.gameObject);
		DisableEngagementPreviewEvent += () => Destroy(miniPreview_Defender.gameObject);

		// visualize certain values, and ensure the previews are reverted when the EngagementPreview proper is disabled
		potentialEngagement.aggressor.GetComponentInChildren<MiniHealthBar>()?.PreviewDamage(
			enemyPreviewStats.finalDamageContext.Max*(potentialEngagement.defender.unitStats._MULTISTRIKE+1)
		);
		potentialEngagement.defender.GetComponentInChildren<MiniHealthBar>()?.PreviewDamage(
			playerPreviewStats.finalDamageContext.Max*(potentialEngagement.aggressor.unitStats._MULTISTRIKE+1)
		);
		DisableEngagementPreviewEvent += () => potentialEngagement.aggressor.GetComponentInChildren<MiniHealthBar>()?.RevertPreview();
		DisableEngagementPreviewEvent += () => potentialEngagement.defender.GetComponentInChildren<MiniHealthBar>()?.RevertPreview();
	}

	public void DisableEngagementPreview() {
		engagementPreviewBar.gameObject.SetActive(false);
		engagementPreviewBar.GetComponent<UIAnchoredSlider>().SetActive(false);

		// invoke and immediately clear invocation list
		// this is to clear all the anon functions we put on this from the MiniPreviews
		DisableEngagementPreviewEvent?.Invoke();
		DisableEngagementPreviewEvent = null;
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