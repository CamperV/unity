using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerUnitController : Controller
{
	private PathOverlayIsoTile pathOverlayTile;

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
		
		pathOverlayTile = PathOverlayIsoTile.GetTileWithSprite(0);

		// this needs to be done at run-time
		actionBindings[KeyCode.Mouse0] = Interact;
		actionBindings[KeyCode.Mouse1] = ClearSelection;
		actionBindings[KeyCode.K]	   = SkipPhase;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.battle;
	}

	public override void Register(MovingObject subject) {
		base.Register(subject);
		//
		Unit unit = subject as Unit;
		unit.parentController = this;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		var kc = CheckInput();
		
		switch(phaseActionState) {
			// always read input in these states
			case Enum.PhaseActionState.waitingForInput:
			case Enum.PhaseActionState.acting:
				if (actionBindings.ContainsKey(kc)) actionBindings[kc]();

				// if we've entered this state as a result of selecting a unit:
				if (activeRegistry.Contains(currentSelection)) {
					if (currentSelection.OptionActive("Move")) {
						// overlay tile for movement selections
						// constantly recalculate the shortest path to mouse via FlowField
						// on mouse down, start a coroutine to move along the path
						DrawValidMoveForSelection(currentSelection.moveRange);
					}
				}

				// finally, check all unit in registry
				// if none of them have any moves remaining, end the phase
				bool endPhaseNow = true;
				foreach (Unit unit in activeRegistry) {
					if (unit.AnyOptionActive()) {
						endPhaseNow = false;
						break;
					}
				}
				if (endPhaseNow) phaseActionState = Enum.PhaseActionState.complete;
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				RefreshAllUnits();
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
				var success = SelectUnit(target);
				if (success) {
					interactState = Enum.InteractState.unitSelected;
				} else {
					Debug.Log($"Couldn't select {target}");
				}
				break;

			case Enum.InteractState.unitSelected:
				// if mouse is down on a current selection - deselect it
				if (currentSelection.gridPosition == target) {
					ClearSelection();
					break;
				}

				// OUR UNIT:
				if (activeRegistry.Contains(currentSelection)) {

					// if the mouseDown is on a valid square, move to it
					if (currentSelection.OptionActive("Move") && currentSelection.moveRange.ValidMove(target)) {
						currentSelection.TraverseTo(target, fieldPath: currentSelectionFieldPath);
						//
						currentSelection.SetOption("Move", false);
						currentSelection.OnDeselect();
						currentSelectionFieldPath?.UnShow(GameManager.inst.tacticsManager.GetActiveGrid());

						var mm = GameManager.inst.mouseManager;
						grid.ResetSelectionAtAlternate(mm.prevMouseGridPos);

						// dumb shenanigans: clear then re-select
						StartCoroutine(currentSelection.ExecuteAfterMoving(() => {
							SelectUnit(currentSelection.gridPosition);
						})); 
						
						//EndTurnSelectedUnit();
						break;
					}

					// if the mouseDown is on a valid attackable are (after moving)
					if (currentSelection.OptionActive("Attack") && currentSelection.attackRange.ValidAttack(currentSelection, target)) {
						AttackUnit(target);
						currentSelection.SetOption("Attack", false);

						EndTurnSelectedUnit();
						break;
					}
				}

				// NOT OUR UNIT
				else {
					Debug.Log("can't really do anything w/ a non-registered unit");
				}

				// default: Select someone else
				SelectUnit(target);
				break;
		}
	}
	
	private bool SelectUnit(Vector3Int target) {
		// on a certain key, get the currently selected unit
		// enter a special controller mode
		var unitAt = (Unit)grid.OccupantAt(target);

		if (unitAt != null)
			Debug.Log($"trying to select {unitAt}, options active?: {unitAt.AnyOptionActive()}");
		if (unitAt == null) return false;

		// if this is any unit at all:
		if (unitAt.AnyOptionActive()) {
			// deselect current and select the new
			if (currentSelection != null) {
				currentSelection.OnDeselect();
			}
			currentSelection = unitAt;
			currentSelection.OnSelect();
		}

		return currentSelection == unitAt;
	}

	private void ClearSelection() {
		if (currentSelection != null) {
			currentSelection.OnDeselect();
			currentSelection = null;
		}
		currentSelectionFieldPath?.UnShow(GameManager.inst.tacticsManager.GetActiveGrid());

		var mm = GameManager.inst.mouseManager;
		grid.ResetSelectionAtAlternate(mm.prevMouseGridPos);

		interactState = Enum.InteractState.noSelection;
	}

	private void EndTurnSelectedUnit() {
		currentSelection.OnDeselect();
		currentSelection.OnEndTurn();
		currentSelection = null;
		ClearSelection();
	}

	private void AttackUnit(Vector3Int target) {
		Debug.Log($"{currentSelection} attacking {target}");
		var unitAt = (Unit)grid.OccupantAt(target);
		currentSelection.Attack(unitAt);
	}

	private void SkipPhase() {
		ClearSelection();
		phaseActionState = Enum.PhaseActionState.complete;
	}
}