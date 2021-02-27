﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerUnitController : UnitController
{
	private PathOverlayIsoTile pathOverlayTile;

	private PlayerUnit currentSelection;
	private MovingObjectPath currentSelectionFieldPath;
	private TacticsGrid grid;

	// possible actions for PlayerUnits and their bindings
	private Dictionary<KeyCode, Action> actionBindings = new Dictionary<KeyCode, Action>();

	private Enum.InteractState interactState;
	
	protected override void Awake() {
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
		PlayerUnit unit = subject as PlayerUnit;
		unit.parentController = this;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		var kc = CheckInput();

		// if the unit's turn was ended some other way, other than through attacking
		if (currentSelection?.turnActive == false) ClearSelection();
		
		switch(phaseActionState) {
			// always read input in these states
			case Enum.PhaseActionState.waitingForInput:
				if (actionBindings.ContainsKey(kc)) actionBindings[kc]();

				// CURRENT SELECTION STATE
				// if we've entered this state as a result of selecting a unit:
				if (activeRegistry.Contains(currentSelection)) {					
					switch (currentSelection?.actionState) {
						case Enum.PlayerUnitState.moveSelection:
							// overlay tile for movement selections
							// constantly recalculate the shortest path to mouse via FlowField
							// on mouse down, start a coroutine to move along the path
							DrawValidMoveForSelection(currentSelection.moveRange);
							break;

						case Enum.PlayerUnitState.attackSelection:
							PreviewPossibleEngagement();
							break;
					}
				}
				// CURRENT SELECTION STATE

				// finally, check all unit in registry
				// if none of them have any moves remaining, end the phase
				if (EndPhaseNow()) phaseActionState = Enum.PhaseActionState.complete;
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase(postPhaseDelay);
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

	private void PreviewPossibleEngagement() {
		Vector3Int target = GetMouseTarget();
		GetOpposing().ForEach( it => (it as Unit).SetMildFocus(false) );

		// TODO: I hate this
		// we are re-drawing the red squares EVERY FRAME THAT WE ARE IN ATTACK SELECTION
		// while it's not really a big deal, I just don't like it
		currentSelection.DisplayStandingThreatRange();

		// if target is an enemy combatant & we are about to attack it
		if (currentSelection.attackRange.ValidAttack(currentSelection, target)) {
			if (GetOpposing().Select(it => it.gridPosition).Contains(target)) {
				// preview the potential engagement here
				var unitAt = grid.OccupantAt(target) as Unit;
				unitAt.SetMildFocus(true);

				if (MenuManager.inst.engagementPreview == null || MenuManager.inst.engagementPreview.defender != unitAt) {	
					var previewEngagement = new Engagement(currentSelection, unitAt);
					EngagementResults er = previewEngagement.PreviewResults();
					MenuManager.inst.CreateEngagementPreview(er);
				}
				return;
			}
		}

		// if didn't return
		MenuManager.inst.DestroyCurrentEngagementPreview();
	}
	// UPDATE ZONE

	// ACTION ZONE
	private void Interact() {
		// this is the contextual interaction
		// get the gridPosition of the targeted click
		// check a unit's box collider too, so that user can click the sprite too
		// ALSO, make sure a ghosted clickable is click-thru only
		Vector3Int target = GetMouseTarget();
		
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
				// if mouse is down on a current selection - bring up the menu and cancel move/attack selection
				if (currentSelection.gridPosition == target) {
					ClearSelection();
					break;
				}

				// OUR UNIT:
				if (activeRegistry.Contains(currentSelection)) {
					//
					switch(((PlayerUnit)currentSelection).actionState) {
						case Enum.PlayerUnitState.moveSelection:
							// if the mouseDown is on a valid square, move to it
							if (currentSelection.moveRange.ValidMove(target)) {
								currentSelection.SetOption("Move", false);

								// unshow some ugly bits as we travel
								currentSelection.UnlockSelection();
								currentSelectionFieldPath?.UnShow(grid);

								currentSelection.TraverseTo(target, fieldPath: currentSelectionFieldPath);

								// on end
								StartCoroutine(currentSelection.ExecuteAfterMoving(() => {
									currentSelection.LockSelection();
									currentSelection.EnterNextState(orEndTurn: true);
								}));
							}
							break;

						case Enum.PlayerUnitState.attackSelection:
							// ON CLICK - target has already been selected
							// if currentSelection can actually attack the target
							if (currentSelection.attackRange.ValidAttack(currentSelection, target)) {

								// if target is an enemy combatant
								if (GetOpposing().Select(it => it.gridPosition).Contains(target)) {
									currentSelection.SetOption("Attack", false);
									//									
									var engagement = new Engagement(currentSelection, (grid.OccupantAt(target) as Unit));
									StartCoroutine(engagement.ResolveResults());

									// wait until the engagement has ended
									// once the engagement has processed, resolve the casualties
									// once the casualties are resolved, EndTurnSelectedUnit()
									StartCoroutine(engagement.ExecuteAfterResolving(() => {
										StartCoroutine(engagement.results.ResolveCasualties());

										// on end
										StartCoroutine(engagement.results.ExecuteAfterResolving(() => {
											EndTurnSelectedUnit();
										}));
									}));
								}
							}
							break;

						default:
							break;
					}	// end case
				}

				// NOT OUR UNIT
				else {
					Debug.Log("can't really do anything w/ a non-registered unit");
				}

				// default: Select someone else if you click on them
				SelectUnit(target);
				break;

			default:
				break;
			// endcase
		}
	}
	
	private bool SelectUnit(Vector3Int target) {
		// on a certain key, get the currently selected unit
		// enter a special controller mode
		var unitAt = (Unit)grid.OccupantAt(target);
		if (unitAt == null || unitAt == currentSelection || !unitAt.turnActive) return false;

		if (activeRegistry.Contains(unitAt)) {
			// deselect current and select the new
			if (currentSelection != null) {
				currentSelection.OnDeselect();
			}
			currentSelection = unitAt as PlayerUnit;
			currentSelection.OnSelect();
			return true;

		// have some polymorphic stuff, like show the movement range or something
		} else {
			return false;
		}
	}

	public void ClearSelection() {
		if (currentSelection != null) {
			currentSelection.OnDeselect();
			currentSelection = null;
		}
		currentSelectionFieldPath?.UnShow(GameManager.inst.tacticsManager.GetActiveGrid());
		interactState = Enum.InteractState.noSelection;
	}

	public void EndTurnSelectedUnit() {
		currentSelection.OnDeselect();
		currentSelection.OnEndTurn();
		currentSelection = null;
		ClearSelection();
	}

	private void SkipPhase() {
		ClearSelection();
		phaseActionState = Enum.PhaseActionState.complete;
	}

	private bool PossibleValidAttack(Unit subject, List<MovingObject> potentialTargets) {
		subject.UpdateThreatRange();
		return potentialTargets.FindAll(
			it => it != subject && subject.attackRange.field.ContainsKey(it.gridPosition)
		).Any();
	}

	private Vector3Int GetMouseTarget() {
		// check bboxes to reset target
		// and if it isn't there, get the gridpos
		var activeBattle = GameManager.inst.tacticsManager.activeBattle;
		var ascendingInBattle = activeBattle.GetRegisteredInBattle().OrderBy(it => it.transform.position.y);

		foreach (TacticsEntityBase clickable in ascendingInBattle) {
			if (!clickable.ghosted && clickable.ColliderContains(GameManager.inst.mouseManager.mouseWorldPos)) {
				return clickable.gridPosition;
			}
		};
		return GameManager.inst.mouseManager.currentMouseGridPos;
	}
}