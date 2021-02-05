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

	protected override void Awake() {
		base.Awake();
		//

		actionState = Enum.PlayerUnitState.idle;
		unitUI.actionMenu.BindCallbacks(new Dictionary<string, Action>(){
			["Move"]   = () => { EnterMoveSelection(); },
			["Attack"] = () => { EnterAttackSelection(); },
			["Wait"]   = () => { ((PlayerUnitController)parentController).EndTurnSelectedUnit(); },
			["Cancel"] = () => { EnterIdleOrClearSelection(); }
		});
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
		EnterState(Enum.PlayerUnitState.menu);
	}

	public override void OnDeselect() {
		//
		SetFocus(false);
		selectionLock = false;
		unitUI.healthBar.Hide();

		EnterState(Enum.PlayerUnitState.idle);
	}

	private void EnterState(Enum.PlayerUnitState state) {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		actionState = state;

		switch(actionState) {
			case Enum.PlayerUnitState.idle:
				unitUI.actionMenu.ClearDisplay();
				//
				moveRange?.ClearDisplay(grid);
				attackRange?.ClearDisplay(grid);
				break;
			case Enum.PlayerUnitState.menu:
				unitUI.actionMenu.Display();
				//
				moveRange?.ClearDisplay(grid);
				attackRange?.ClearDisplay(grid);
				break;
			case Enum.PlayerUnitState.moveSelection:
				//unitUI.actionMenu.ClearDisplay();
				//
				UpdateThreatRange();
				attackRange?.Display(grid);
				moveRange?.Display(grid);
				break;
			case Enum.PlayerUnitState.attackSelection:
				//unitUI.actionMenu.ClearDisplay();
				//
				UpdateThreatRange();
				attackRange?.Display(grid);
				moveRange?.Display(grid);
				break;
		}
	}
	public void EnterIdleOrClearSelection() {
		switch (actionState) {
			case Enum.PlayerUnitState.moveSelection:
			case Enum.PlayerUnitState.attackSelection:
				EnterState(Enum.PlayerUnitState.idle);
				break;
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
