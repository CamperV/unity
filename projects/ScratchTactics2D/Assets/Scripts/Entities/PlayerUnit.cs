using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public abstract class PlayerUnit : Unit
{
	// NOTE this is set by a Controller during registration
	//public override PlayerUnitController parentController;
	//public override bool isPlayerControlled { get => true; }

	//
	public Enum.PlayerUnitState actionState;
	public Dictionary<string, Action> callbackBindings;

	protected override void Awake() {
		base.Awake();
		//
		callbackBindings = new Dictionary<string, Action>(){
			["MoveButton"]   = () => { EnterMoveSelection(); },
			["AttackButton"] = () => { EnterAttackSelection(); },
			["WaitButton"]   = () => { ((PlayerUnitController)parentController).EndTurnSelectedUnit(); },
			["CancelButton"] = () => { EnterIdleOrClearSelection(); }
		};
		actionState = Enum.PlayerUnitState.idle;
	}

	// Action zone
	public override void OnStartTurn() {	
		optionAvailability.Keys.ToList().ForEach(k => optionAvailability[k] = true);
		spriteRenderer.color = Color.white;
		//
		actionState = Enum.PlayerUnitState.idle;
		turnActive = true;
	}

	public override void OnEndTurn() {
		optionAvailability.Keys.ToList().ForEach(k => optionAvailability[k] = false);
		spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, spriteRenderer.color.a);
		//
		actionState = Enum.PlayerUnitState.idle;
		turnActive = false;
	}

	public override void OnSelect() {
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// 
		SetFocus(true);
		selectionLock = true;
		unitUI.healthBar.Show(true);

		// if you haven't moved yet, enter moveSelection state
		// otherwise, show the menu
		// the menu will determine if the unit enters attackSelection or specialSelection (not yet implemented)
		// enter the first default state for move selection
		if (OptionActive("Move")) {
			EnterState(Enum.PlayerUnitState.moveSelection);
		} else if (OptionActive("Attack")) {
			EnterState(Enum.PlayerUnitState.attackSelection);
		} else {
			EnterMenu();
		}
	}

	public override void OnDeselect() {
		//
		SetFocus(false);
		selectionLock = false;
		unitUI.healthBar.Hide();

		EnterState(Enum.PlayerUnitState.idle);
	}

	private void EnterState(Enum.PlayerUnitState state) {
		var ab = GameManager.inst.tacticsManager.activeBattle;
		GameManager.inst.tacticsManager.virtualCamera.ReleaseLock(ab);
		actionState = state;

		switch(actionState) {
			case Enum.PlayerUnitState.idle:
				MenuManager.inst.DestroyCurrentActionPane();
				//
				ClearDisplayThreatRange();
				break;
			case Enum.PlayerUnitState.menu:
				MenuManager.inst.CreateActionPane(this, callbackBindings);
				MenuManager.inst.actionPane.StopAllButtonPulse();
				//
				ClearDisplayThreatRange();
				break;
			case Enum.PlayerUnitState.moveSelection:
				MenuManager.inst.CreateActionPane(this, callbackBindings);
				MenuManager.inst.actionPane.PulseButton("MoveButton", true);
				//
				DisplayThreatRange();
				break;
			case Enum.PlayerUnitState.attackSelection:
				MenuManager.inst.CreateActionPane(this, callbackBindings);
				MenuManager.inst.actionPane.PulseButton("AttackButton", true);

				// need to change how attack selection/display works
				// because right now
				DisplayStandingThreatRange();

				GameManager.inst.tacticsManager.virtualCamera.ZoomToAndLock(ab, transform.position);
				break;
		}
	}
	public void EnterIdleOrClearSelection() {
		switch (actionState) {
			default:
				EnterState(Enum.PlayerUnitState.idle);
				((PlayerUnitController)parentController).ClearSelection();
				break;
		}
	}
	public void EnterMenu() 		   { EnterState(Enum.PlayerUnitState.menu); }	
	public void EnterMoveSelection()   { EnterState(Enum.PlayerUnitState.moveSelection); }
	public void EnterAttackSelection() { EnterState(Enum.PlayerUnitState.attackSelection); }
	// callbacks
}
