using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	// possible actions for Player and their bindings
	private Dictionary<KeyCode, Func<Army, int>> actionBindings = new Dictionary<KeyCode, Func<Army, int>>();
	
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
	
	private IEnumerator SubjectsTakeAction(Func<Army, int> Action) {
		Army lastSubject = activeRegistry[0] as Army;
		foreach (var _subject in activeRegistry) {
			Army subject = _subject as Army;

			// we've taken an action... but what did it cost
			int ticksTaken = Action(subject);
			//
			GameManager.inst.enemyController.AddTicksAll(ticksTaken);

			lastSubject = subject;
			yield return new WaitForSeconds(phaseDelayTime);
		}
		
		StartCoroutine( lastSubject.ExecuteAfterMoving( () => {
			phaseActionState = Enum.PhaseActionState.complete;
		}) );
	}

	// these Funcs return the cost of taking said actions
	private int MoveLeft(Army subject) {
		var success = subject.GridMove(-5, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveRight(Army subject) {
		var success = subject.GridMove(5, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveUp(Army subject) {
		var success = subject.GridMove(0, 5);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveDown(Army subject) {
		var success = subject.GridMove(0, -5);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int Pass(Army subject) {
		return Constants.standardTickCost;
	}
}