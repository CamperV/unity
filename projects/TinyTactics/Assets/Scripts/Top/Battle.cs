using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(UnitMap), typeof(TurnManager))]
public class Battle : MonoBehaviour
{
    public UnityEvent OnBattleStart;
    public UnityEvent OnBattleEnd;

    [SerializeField] private UnitMap unitMap;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private PlayerUnitController playerUnitController;
    [SerializeField] private EnemyUnitController enemyUnitController;
    [SerializeField] private BattleJukeBox jukeBox;

    public void StartBattle() {
        OnBattleStart?.Invoke();
    }

    public void EndBattle(bool playerVictorious) {        
        turnManager.currentPhase.TriggerEnd();

        // just in case...
        BroadcastMessage("StopAllCoroutines");

        int survivingUnits = playerUnitController.GetActiveUnits().Count;
        int turnsElapsed = turnManager.turnCount;

        if (playerVictorious) {
            jukeBox.SwitchToVictoryTrack(0.25f);
            UIManager.inst.CreateVictoryPanel(-1, survivingUnits, turnsElapsed);
        } else {
            jukeBox.SwitchToDefeatTrack(0.25f);
            UIManager.inst.CreateDefeatPanel(-1, survivingUnits, turnsElapsed);
        }

        //
        OnBattleEnd?.Invoke();
    }

    public void CheckVictoryConditions() {
        // the main victory conditions is defeating all enemy units
        bool enemyUnitsAlive = enemyUnitController.GetActiveUnits().Any();

        if (!enemyUnitsAlive) {
            EndBattle(true);
        }
    }

    public void CheckDefeatConditions() {
        // the main defeat condition is losing all of your units
        bool playerUnitsAlive = playerUnitController.GetActiveUnits().Any();

        if (!playerUnitsAlive) {
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
            clonedUnit.ImportData(unitData);

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
