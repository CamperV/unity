using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(UnitMap), typeof(TurnManager))]
public class Battle : MonoBehaviour
{
    public delegate void BattleEvent();
    public event BattleEvent BattleStartEvent;
    
    public delegate void BattleEndEvent(bool playerVictorious);
    public event BattleEndEvent ConditionalBattleEndEvent;

    private EventManager eventManager;
    private UnitMap unitMap;
    private TurnManager turnManager;
    
    private PlayerUnitController playerUnitController;
    private EnemyUnitController enemyUnitController;

    [SerializeField] private BattleJukeBox jukeBox;

    void Awake() {
        eventManager = GetComponent<EventManager>();
        unitMap = GetComponent<UnitMap>();
        turnManager = GetComponent<TurnManager>();

        playerUnitController = GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = GetComponentInChildren<EnemyUnitController>();
    }

    public void StartBattle() {
        eventManager.EnablePlayerInput();
        eventManager.RegisterEvents();
        //
        BattleStartEvent?.Invoke();
    }

    public void EndBattle(bool playerVictorious) {
        eventManager.DisablePlayerInput();
        
        turnManager.currentPhase.TriggerEnd();
        turnManager.Disable();

        // just in case...
        BroadcastMessage("StopAllCoroutines");

        int enemiesDefeated = enemyUnitController.disabledUnits.Count;
        int survivingUnits = playerUnitController.activeUnits.Count;
        int turnsElapsed = turnManager.turnCount;

        if (playerVictorious) {
            jukeBox.SwitchToVictoryTrack(0.25f);
            UIManager.inst.CreateVictoryPanel(enemiesDefeated, survivingUnits, turnsElapsed);
        } else {
            jukeBox.SwitchToDefeatTrack(0.25f);
            UIManager.inst.CreateDefeatPanel(enemiesDefeated, survivingUnits, turnsElapsed);
        }

        //
        ConditionalBattleEndEvent?.Invoke(playerVictorious);
    }

    public void CheckVictoryConditions() {
        // the main victory conditions is defeating all enemy units
        bool enemyUnitsAlive = enemyUnitController.activeUnits.Any();

        if (!enemyUnitsAlive) {
            UIManager.inst.combatLog.AddEntry($"GREEN@[VICTORY!]");
            EndBattle(true);
        }
    }

    public void CheckDefeatConditions() {
        // the main defeat condition is losing all of your units
        bool playerUnitsAlive = playerUnitController.activeUnits.Any();

        if (!playerUnitsAlive) {
            UIManager.inst.combatLog.AddEntry($"RED@[Y O U  D I E D]");
            EndBattle(false);
        }
    }

    public void ImportCampaignData(ICollection<CampaignUnitGenerator.CampaignUnitData> serializedUnits) {
        List<PlayerUnit> instantiatedUnits = new List<PlayerUnit>();

        // load all specified Prefabs from Resources.
        // the Campaign will track anything you need: only load prefabs to actually give a base GameObject to work upon
        foreach (CampaignUnitGenerator.CampaignUnitData unitData in serializedUnits) {
            PlayerUnit loadedPrefab = Resources.Load<PlayerUnit>($"Units/PlayerUnits/{unitData.className}");
            PlayerUnit clonedUnit = Instantiate(loadedPrefab, playerUnitController.transform);

			clonedUnit.unitStats.ApplyNature(unitData.nature);

			foreach (PerkData perkData in unitData.perks) {
				Type perkType = Type.GetType(perkData.typeName);
				clonedUnit.gameObject.AddComponent(perkType);
			}

            //
            playerUnitController.RegisterUnit(clonedUnit);
            instantiatedUnits.Add(clonedUnit);
        }

        // get them into the unitMap
		unitMap.InsertUnitsAtSpawnMarkers(instantiatedUnits);

        // after using them, get rid of those pesky things
        foreach (SpawnMarker sm in playerUnitController.GetComponentsInChildren<SpawnMarker>()) {
            Destroy(sm.gameObject);
        }
    }
}
