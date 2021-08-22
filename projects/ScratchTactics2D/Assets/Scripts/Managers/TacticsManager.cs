using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class TacticsManager : MonoBehaviour
{
	public Battle battlePrefab;
	[HideInInspector] public Battle activeBattle;

    void Update() {
		// while "in-battle" wait for key commands to exit state
		if (GameManager.inst.gameState == Enum.GameState.battle) {

			// prematurely destroy battle
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("Exiting Battle...");
				activeBattle.Destroy();
			}
		}
    }
	
	public TacticsGrid GetActiveGrid() {
		Debug.Assert(GameManager.inst.gameState == Enum.GameState.battle);
		return activeBattle.grid;
	}
	
	// handles construction of Battle and management of tacticsGrid
	// NOTE: we can only ever start a battle with two participants
	// because of the interleaving of Overworld turns and Tactics turns, even if there WOULD be more than one enemy active,
	// it still won't be included in the battle until it can take its turn
	public void CreateActiveBattle(PlayerArmy player, EnemyArmy other, Terrain playerTerrain, Terrain otherTerrain, Enum.Phase initiatingPhase) {
		var cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);		
		activeBattle = Instantiate(battlePrefab, cameraPos, Quaternion.identity);
		activeBattle.Init(player, other);
		activeBattle.LoadBattleMap(playerTerrain, otherTerrain);
		activeBattle.SpawnAllUnits();
		activeBattle.PostInit();

		activeBattle.StartBattleOnPhase(initiatingPhase);
	}

	public void AddToActiveBattle(EnemyArmy other, Terrain otherTerrain) {
		activeBattle.AddParticipant(other, otherTerrain);
	}
}
