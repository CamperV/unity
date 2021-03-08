using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	// possible actions for Player and their bindings
	private Dictionary<KeyCode, Func<MovingObject, int>> actionBindings = new Dictionary<KeyCode, Func<MovingObject, int>>();
	
	protected override void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;
		
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
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.overworld;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		var kc = CheckInput();
		
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				if (actionBindings.ContainsKey(kc)) {
					phaseActionState = Enum.PhaseActionState.acting;
					StartCoroutine(SubjectsTakeAction(actionBindings[kc]));
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
	
	private IEnumerator SubjectsTakeAction(Func<MovingObject, int> action) {
		foreach (var subject in activeRegistry) {
			// we've taken an action... but what did it cost
			int ticksTaken = action(subject);
			//
			GameManager.inst.enemyController.AddTicksAll(ticksTaken);
			yield return new WaitForSeconds(phaseDelayTime);
		}
		
		phaseActionState = Enum.PhaseActionState.complete;
	}

	// these Funcs return the cost of taking said actions
	private int MoveLeft(MovingObject subject) {
		var success = subject.GridMove(-1, 0);
		var tickCost = GameManager.inst.worldGrid.GetTileAt(subject.gridPosition).cost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveRight(MovingObject subject) {
		var success = subject.GridMove(1, 0);
		var tickCost = GameManager.inst.worldGrid.GetTileAt(subject.gridPosition).cost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveUp(MovingObject subject) {
		var success = subject.GridMove(0, 1);
		var tickCost = GameManager.inst.worldGrid.GetTileAt(subject.gridPosition).cost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveDown(MovingObject subject) {
		var success = subject.GridMove(0, -1);
		var tickCost = GameManager.inst.worldGrid.GetTileAt(subject.gridPosition).cost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int Pass(MovingObject subject) {
		return Constants.standardTickCost;
	}
}