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
			["WaitButton"]   = () => { Wait(); },
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
		LockSelection();
		EnterNextState();
	}

	public override void OnDeselect() {
		UnlockSelection();
		EnterState(Enum.PlayerUnitState.idle);
	}

	public void LockSelection() {
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		SetFocus(true);
		selectionLock = true;
		//unitUI.healthBar.Show(true);
	}
	
	public void UnlockSelection() {
		SetFocus(false);
		selectionLock = false;
		//unitUI.healthBar.Hide();
	}

	private void EnterState(Enum.PlayerUnitState state) {
		GameManager.inst.tacticsManager.virtualCamera.ReleaseLock();
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

				GameManager.inst.tacticsManager.virtualCamera.ZoomToAndLock(transform.position, 1.40f);
				break;
		}
	}

	// callbacks
	public void EnterIdleOrClearSelection() {
		switch (actionState) {
			default:
				EnterState(Enum.PlayerUnitState.idle);
				(parentController as PlayerUnitController).ClearSelection();
				break;
		}
	}
	public void EnterMenu() {
		if (actionState != Enum.PlayerUnitState.menu) {
			EnterState(Enum.PlayerUnitState.menu);
		}
	}
	public void EnterMoveSelection() {
		if (actionState != Enum.PlayerUnitState.moveSelection) {
			EnterState(Enum.PlayerUnitState.moveSelection);
		}
	}
	public void EnterAttackSelection() {
		if (actionState != Enum.PlayerUnitState.attackSelection) {
			EnterState(Enum.PlayerUnitState.attackSelection);
		}
	}
	public void Wait() {
		(parentController as PlayerUnitController).EndTurnSelectedUnit();
	}
	// callbacks

	public void EnterNextState(bool orEndTurn = false) {
		// STATE FLOWCHART
		// if you haven't moved yet, enter moveSelection state
		// otherwise, show the menu
		// the menu will determine if the unit enters attackSelection or specialSelection (not yet implemented)
		// enter the first default state for move selection
		if (OptionActive("Move")) {
			EnterMoveSelection();

		} else if (OptionActive("Attack") && ValidAttackExists()) {
			EnterAttackSelection();

		} else {
			if (orEndTurn) {
				Wait();
			} else {
				EnterMenu();
			}
		}
	}

	public bool ValidAttackExists() {
		return (parentController as PlayerUnitController).GetOpposing().Where( it => InStandingAttackRange(it.gridPosition) ).Any();
	}
}
