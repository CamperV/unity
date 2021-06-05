﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	// possible actions for Player and their bindings
	private Dictionary<KeyCode, Func<Army, int>> actionBindings = new Dictionary<KeyCode, Func<Army, int>>();
	private Dictionary<Vector3Int, Func<Army, int>> directionMapping = new Dictionary<Vector3Int, Func<Army, int>>();

	public Queue<Func<Army, int>> actionQueue;
	private OverworldPlayer registeredPlayer { get => activeRegistry[0] as OverworldPlayer; }
	
	protected override void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;
		actionQueue = new Queue<Func<Army, int>>();
		
		// this needs to be done at run-time
		// is this  abad idiom? Or is this more just for organization?
		actionBindings[KeyCode.LeftArrow]  = MoveLeft;
		actionBindings[KeyCode.RightArrow] = MoveRight;
		actionBindings[KeyCode.UpArrow]    = MoveUp;
		actionBindings[KeyCode.DownArrow]  = MoveDown;
		actionBindings[KeyCode.A]  		   = MoveLeft;
		actionBindings[KeyCode.D] 		   = MoveRight;
		actionBindings[KeyCode.W]    	   = MoveUp;
		actionBindings[KeyCode.S]  		   = MoveDown;
		actionBindings[KeyCode.Space]	   = Pass;

		// for pathToQueue lookup
		directionMapping[Vector3Int.left]  = MoveLeft;
		directionMapping[Vector3Int.right] = MoveRight;
		directionMapping[Vector3Int.up]    = MoveUp;
		directionMapping[Vector3Int.down]  = MoveDown;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.overworld;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		KeyCode kc = CheckInput();
		
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				// if the mouse was pressed on the Overworld
				if (Input.GetMouseButtonDown(0)) {
					Vector3Int mousePos = GameManager.inst.overworld.Real2GridPos(GameManager.inst.mouseManager.mouseWorldPos);
					Path pathToQueue = new ArmyPathfinder().BFS(registeredPlayer.gridPosition, mousePos);

					foreach (Vector3Int nextMove in pathToQueue.Serially()) {
						actionQueue.Enqueue(directionMapping[nextMove]);
					}

				// or, if there was a relevant keypress
				} else if (actionBindings.ContainsKey(kc)) {
					actionQueue.Enqueue(actionBindings[kc]);
				}
				
				// now unspool the queue
				if (actionQueue.Count > 0) {
					Func<Army, int> actionToTake = actionQueue.Dequeue();

					phaseActionState = Enum.PhaseActionState.acting;
					StartCoroutine( PlayerTakeAction(actionToTake) );
				}
				break;
				
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase();
				break;
			
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:	
			case Enum.PhaseActionState.postPhase:
				break;
		}
    }

	private KeyCode CheckInput() {
		// return KeyCode that is down, checking in "actionBindings" order
		foreach (KeyCode kc in actionBindings.Keys) {
			if (Input.GetKeyDown(kc)) return kc; 
		}
		return KeyCode.None;
	}
	
	private IEnumerator PlayerTakeAction(Func<Army, int> Action) {
		// we've taken an action... but what did it cost
		int ticksTaken = Action(registeredPlayer);
		GameManager.inst.enemyController.AddTicksAll(ticksTaken);

		yield return new WaitForSeconds(phaseDelayTime);

		StartCoroutine( registeredPlayer.ExecuteAfterMoving( () => {
			phaseActionState = Enum.PhaseActionState.complete;
		}) );
	}

	// these Funcs return the cost of taking said actions
	private int MoveLeft(Army subject) {
		var success = subject.GridMove(-1, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveRight(Army subject) {
		var success = subject.GridMove(1, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveUp(Army subject) {
		var success = subject.GridMove(0, 1);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveDown(Army subject) {
		var success = subject.GridMove(0, -1);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int Pass(Army subject) {
		return Constants.standardTickCost;
	}
}