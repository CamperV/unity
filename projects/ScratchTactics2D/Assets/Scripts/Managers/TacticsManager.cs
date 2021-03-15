﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class TacticsManager : MonoBehaviour
{
	public Battle battlePrefab;
	[HideInInspector] public Battle activeBattle;

	public VirtualCamera virtualCamera;
	public bool resizeLock;

	// only one Unit can be in focus at any given time
	public Unit focusSingleton { get; private set; }
	
	void Awake() {
		resizeLock = false;
	}

    void Update() {
		// while "in-battle" wait for key commands to exit state
		if (GameManager.inst.gameState == Enum.GameState.battle) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("Exiting Battle...");
				
				DestroyActiveBattle();
			}

			// virtualCamera will have its own battle ref
			if (GameManager.inst.phaseManager.currentPhase == Enum.Phase.player && !resizeLock) {
				virtualCamera.DragUpdate();
				virtualCamera.ScrollUpdate();
			}

			// focus control:
			// move it all into once-per-frame centralized check, because we can't guarantee 
			// the order in which the other Update()/LateUpdate()s resolve
			Unit newFocus = GetNewFocus();

			// actually set the focus here
			// switch focus if the current focusSingleton is null and no selectionLock is in place
			if (newFocus != focusSingleton) {
				if (focusSingleton == null || !focusSingleton.selectionLock) {
					focusSingleton?.SetFocus(false);
					focusSingleton = newFocus;
					focusSingleton?.SetFocus(true);
				}
			}

			//
			// finally, Ghost Control
			var grid = GetActiveGrid();
			var descendingInBattle = activeBattle.GetRegisteredInBattle().OrderByDescending(it => it.gridPosition.y);

			foreach (Unit u in descendingInBattle) {
				u.ghosted = false;
				if (!u.inFocus && !u.inMildFocus) {
					// each unit will check its own processes to see if it should be ghosted
					// having multiple senders, i.e. PathOverlayIso tiles and other Units, is difficult to keep track of

					// if there is any overlay that can be obscured:
					Vector3Int northPos = u.gridPosition + new Vector3Int(1, 1, 0);
					if (grid.GetOverlayAt(u.gridPosition) || grid.GetOverlayAt(northPos)) {
						u.ghosted = true;
					}
							
					// or, if there is a Unit with an active focus right behind
					if (((Unit)grid.OccupantAt(northPos))?.inFocus ?? false) {
						u.ghosted = true;
					}
				}
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
	public void CreateActiveBattle(OverworldPlayer player, OverworldEnemyBase other, WorldTile playerTile, WorldTile enemyTile, Enum.Phase initiatingPhase) {
		var cameraPos = new Vector3(Camera.main.transform.position.x,
									Camera.main.transform.position.y,
									0);		
		activeBattle = Instantiate(battlePrefab, cameraPos, Quaternion.identity);
		activeBattle.Init(player, other, playerTile, enemyTile);
		//
		virtualCamera = new VirtualCamera(activeBattle);
		virtualCamera.Zoom(1.5f);

		activeBattle.StartBattleOnPhase(initiatingPhase);
	}

	public void AddToActiveBattle(OverworldEnemyBase other, WorldTile otherTile) {
		activeBattle.AddParticipant(other, otherTile);
	}

	public void ResolveActiveBattle(OverworldEntity defeatedEntity) {
		defeatedEntity.Die();
		DestroyActiveBattle();
	}
	
	public void DestroyActiveBattle() {
		Destroy(activeBattle.grid.gameObject);
		Destroy(activeBattle.gameObject);
		//
		MenuManager.inst.CleanUpBattleMenus();
		//
		UIManager.inst.EnableBattlePhaseDisplay(false);
		GameManager.inst.EnterOverworldState();
	}

	public Unit GetNewFocus() {
		var mm = GameManager.inst.mouseManager;

		// Focus control: reset if applicable and highlight/focus
		var unitsInBattle = activeBattle.GetRegisteredInBattle().OrderBy(it => it.gridPosition.y);
		foreach (Unit u in unitsInBattle) {
			if (!u.ghosted && u.ColliderContains(mm.mouseWorldPos)) {
				return u;
			}
		}
		
		// secondary try: select based on tileGridPos AFTER determining BB collisions
		foreach (Unit u in unitsInBattle) {
			if (!u.IsMoving() && mm.currentMouseGridPos == u.gridPosition) {
				return u;
			}
		}

		return null;
	}
}
