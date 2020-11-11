using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	// possible actions for Player and their bindings
	private Dictionary<KeyCode, Action<MovingObject>> actionBindings = new Dictionary<KeyCode, Action<MovingObject>>();
	
	void Awake() {
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
	
	private IEnumerator SubjectsTakeAction(Action<MovingObject> action) {
		foreach (var subject in registry) {
			action(subject);
			yield return new WaitForSeconds(phaseDelayTime);
		}
		
		phaseActionState = Enum.PhaseActionState.complete;
	}

	private void MoveLeft(MovingObject subject) 	{ subject.GridMove(-1, 0); }
	private void MoveRight(MovingObject subject) 	{ subject.GridMove(1, 0); }
	private void MoveUp(MovingObject subject) 		{ subject.GridMove(0, 1); }
	private void MoveDown(MovingObject subject) 	{ subject.GridMove(0, -1); }
	private void Pass(MovingObject subject) 		{ return; }

}