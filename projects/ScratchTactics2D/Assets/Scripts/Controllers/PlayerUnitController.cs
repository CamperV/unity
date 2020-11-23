using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerUnitController : Controller
{
	private Unit currentSelection;
	private TacticsGrid grid;

	// possible actions for PlayerUnits and their bindings
	private Dictionary<KeyCode, Action> actionBindings = new Dictionary<KeyCode, Action>();
	
	void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;
		grid = GameManager.inst.tacticsManager.GetActiveGrid();
		
		// this needs to be done at run-time
		actionBindings[KeyCode.Mouse0] = SelectUnit;
		actionBindings[KeyCode.Mouse1] = DeselectUnit;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.battle;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		var kc = CheckInput();
		
		switch(phaseActionState) {
			// always read input in these states
			case Enum.PhaseActionState.waitingForInput:
				if (actionBindings.ContainsKey(kc)) actionBindings[kc]();
				break;

			case Enum.PhaseActionState.acting:
				if (actionBindings.ContainsKey(kc)) actionBindings[kc]();

				// if we've entered this state as a result of selecting a unit:
				if (currentSelection) {
					// overlay tile for movement selections
					// constantly recalculate the shortest path to mouse via FlowField
					// on mouse down, start a coroutine to move along the path
				}
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

	private void DrawToMoveTile(FlowField ffield) {
		var mm = GameManager.inst.mouseManager;
		//
		if (mm.HasMouseMovedGrid()) {		
			grid.ResetSelectionAt(mm.prevMouseGridPos);
			if (grid.IsInBounds(mm.currentMouseGridPos)) {
				grid.SelectAt(mm.currentMouseGridPos);
			}
		}
	}

	// ACTION ZONE
	private void SelectUnit() {
		// on a certain key, get the currently selected unit
		// enter a special controller mode
		Vector3Int selected = GameManager.inst.mouseManager.currentMouseGridPos;
		//
		var unitAt = (Unit)grid.OccupantAt(selected);
		if (registry.Contains(unitAt)) {
			if (currentSelection != null) {
				currentSelection.OnDeselect();
			}
			currentSelection = unitAt;
			currentSelection.OnSelect();
		}

		// hold UnitController in "acting" state until we cancel or take a unit action
		phaseActionState = Enum.PhaseActionState.acting;
	}

	private void DeselectUnit() {
		if (currentSelection != null) {
			currentSelection.OnDeselect();
			currentSelection = null;

			phaseActionState = Enum.PhaseActionState.waitingForInput;
		}
	}
}