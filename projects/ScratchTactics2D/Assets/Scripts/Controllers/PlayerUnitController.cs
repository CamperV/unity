using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerUnitController : Controller
{
	private PathOverlayTile pathOverlayTile;



	private Unit currentSelection;
	private MovingObjectPath currentSelectionFieldPath;
	private TacticsGrid grid;

	// possible actions for PlayerUnits and their bindings
	private Dictionary<KeyCode, Action> actionBindings = new Dictionary<KeyCode, Action>();

	private Enum.InteractState interactState;
	
	void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;
		grid = GameManager.inst.tacticsManager.GetActiveGrid();
		
		pathOverlayTile = ScriptableObject.CreateInstance<PathOverlayTile>() as PathOverlayTile;

		// this needs to be done at run-time
		actionBindings[KeyCode.Mouse0] = Interact;
		actionBindings[KeyCode.Mouse1] = ClearInteract;
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
					DrawValidMoveForSelection(currentSelection.range);
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

	private void DrawValidMoveForSelection(MoveRange mRange) {
		var mm = GameManager.inst.mouseManager;
		//
		if (mm.HasMouseMovedGrid()) {		
			grid.ResetSelectionAtAlternate(mm.prevMouseGridPos);
			currentSelectionFieldPath?.UnShow(grid);

			// while the origin is a ValidMove, don't draw it
			if (mm.currentMouseGridPos != mRange.origin && mRange.ValidMove(mm.currentMouseGridPos)) {
				grid.SelectAtAlternate(mm.currentMouseGridPos);

				// update this every time you move the mouse. Run time intensive? But shows path taken
				currentSelectionFieldPath = MovingObjectPath.GetPathFromField(mm.currentMouseGridPos, mRange);
				currentSelectionFieldPath.Show(grid, pathOverlayTile);
			}
		}
	}

	// ACTION ZONE
	private void Interact() {
		// this is the contextual interaction
		// 
		Vector3Int target = GameManager.inst.mouseManager.currentMouseGridPos;
		
		switch (interactState) {
			case Enum.InteractState.noSelection:
				SelectUnit(target);
				interactState = Enum.InteractState.unitSelected;
				break;

			case Enum.InteractState.unitSelected:
				// available:	MoveUnit
				//				Attack?
				//				Wait?
				//				Bring up Menu?

				// if the mouseDown is on a valid square, move to it
				if (currentSelection.range.ValidMove(target)) {
					MoveSelectedUnit(target);
					ClearInteract();
				}
				break;
		}

		// hold UnitController in "acting" state until we cancel or take a unit action
		phaseActionState = Enum.PhaseActionState.acting;
	}

	private void ClearInteract() {
		DeselectUnit();

		var mm = GameManager.inst.mouseManager;
		grid.ResetSelectionAtAlternate(mm.prevMouseGridPos);

		interactState = Enum.InteractState.noSelection;
		phaseActionState = Enum.PhaseActionState.waitingForInput;
	}

	private void SelectUnit(Vector3Int target) {
		// on a certain key, get the currently selected unit
		// enter a special controller mode
		var unitAt = (Unit)grid.OccupantAt(target);
		if (registry.Contains(unitAt)) {
			// if trying to select the current selection, just deselect it
			if (currentSelection == unitAt) {
				ClearInteract();
				return;
			}

			// else, deselect current and select the new
			if (currentSelection != null) {
				currentSelection.OnDeselect();
			}
			currentSelection = unitAt;
			currentSelection.OnSelect();
		}
	}

	private void DeselectUnit() {
		if (currentSelection != null) {
			currentSelection.OnDeselect();
			currentSelection = null;
		}
	}

	private void MoveSelectedUnit(Vector3Int target) {
		Debug.Assert(currentSelection != null);
		currentSelection.TraverseTo(target, fieldPath: currentSelectionFieldPath);
	}
}