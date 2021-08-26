using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerUnitController : UnitController
{
	private PlayerUnit currentSelection;
	private BattlePath currentSelectionFieldPath;
	private TacticsGrid grid { get => Battle.active.grid; }

	// possible actions for PlayerUnits and their bindings
	private Dictionary<KeyCode, Action> actionBindings = new Dictionary<KeyCode, Action>();
	private Enum.InteractState interactState;
	
	protected override void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;

		// this needs to be done at run-time
		actionBindings[KeyCode.Mouse0] = Interact;
		actionBindings[KeyCode.Mouse1] = ClearSelection;
		actionBindings[KeyCode.K]	   = SkipPhase;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.battle && Battle.active.interactable;
	}

	public override void Register(MovingGridObject subject) {
		base.Register(subject);
		//
		PlayerUnit unit = subject as PlayerUnit;
		unit.parentController = this;
		
		// init threat for other phases
		unit.UpdateThreatRange();
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
							if (!currentSelection.spriteAnimator.isMoving)
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

	private Vector3Int moveSelectionGridPos;
	private void DrawValidMoveForSelection(MoveRange mRange) {
		currentSelectionFieldPath?.UnShow();

		Vector3Int currentGridPos = grid.GetMouseToGridPos();

		// uncomment this to always be able to move even when the mouse target is not on the grid
		// moveSelectionGridPos = (mRange.ValidMove(currentGridPos)) ? currentGridPos : moveSelectionGridPos;
		moveSelectionGridPos = currentGridPos;

		// while the origin is a ValidMove, don't draw it
		if (mRange.ValidMove(moveSelectionGridPos)) {
			grid.SelectAtAlternate(moveSelectionGridPos);

			// update this every time you move the mouse. Run time intensive? But shows path taken
			currentSelectionFieldPath = new FlowFieldPathfinder(mRange).BFS<BattlePath>(moveSelectionGridPos);
			currentSelectionFieldPath.Show();
		}
	}

	private void PreviewPossibleEngagement() {
		GetOpposing().ForEach( it => (it as Unit).SetMildFocus(false) );

		// TODO: I hate this
		// we are re-drawing the red squares EVERY FRAME THAT WE ARE IN ATTACK SELECTION
		// while it's not really a big deal, I just don't like it
		currentSelection.DisplayStandingThreatRange();
		Vector3Int target = GetMouseTarget();

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
					// ClearSelection();
					
					switch(((PlayerUnit)currentSelection).actionState) {
						case Enum.PlayerUnitState.idle:
							break;
						case Enum.PlayerUnitState.menu:
							break;
						case Enum.PlayerUnitState.moveSelection:
							if (currentSelection.OptionActive("Attack")) {
								currentSelection.EnterAttackSelection();
							}
							break;
						case Enum.PlayerUnitState.attackSelection:
							ClearSelection();
							// if (currentSelection.OptionActive("Move")) {
							// 	currentSelection.EnterMoveSelection();
							// } else {
							// 	ClearSelection();
							// }
							break;
					}
					break;
				}

				// else, the mouse is down on something that isn't the currently selected unit:
				if (activeRegistry.Contains(currentSelection)) {
					//
					switch(((PlayerUnit)currentSelection).actionState) {
						case Enum.PlayerUnitState.moveSelection:
							// if the mouseDown is on a valid square, move to it
							if (currentSelection.moveRange.ValidMove(moveSelectionGridPos)) {
								currentSelection.SetOption("Move", false);

								// unshow some ugly bits as we travel
								currentSelection.UnlockSelection();
								currentSelectionFieldPath?.UnShow();

								currentSelection.TraverseTo(moveSelectionGridPos, currentSelectionFieldPath);

								// on end
								StartCoroutine( currentSelection.spriteAnimator.ExecuteAfterMoving(() => {
									currentSelection.LockSelection();
									currentSelection.EnterNextState(orEndTurn: true);
								}));
							}
							break;

						case Enum.PlayerUnitState.attackSelection:
							// ON CLICK - target has already been selected
							// if currentSelection can actually attack the target
							if (CanAttackTarget(currentSelection, target)) {
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
										MenuManager.inst.DestroyCurrentEngagementPreview();
									}));
								}));
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
		currentSelectionFieldPath?.UnShow();
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

	private bool PossibleValidAttack(Unit subject, List<MovingGridObject> potentialTargets) {
		subject.UpdateThreatRange();
		return potentialTargets.FindAll(
			it => it != subject && subject.attackRange.field.ContainsKey(it.gridPosition)
		).Any();
	}

	private bool CanAttackTarget(PlayerUnit s, Vector3Int target) {
		return s.OptionActive("Attack") && s.attackRange.ValidAttack(s, target) && GetOpposing().Select(it => it.gridPosition).Contains(target);
	}

	private Vector3Int GetMouseTarget() {
		// check bboxes to reset target
		// and if it isn't there, get the gridpos
		var unitsInBattle = Battle.active.RegisteredUnits.OrderBy(it => it.gridPosition.y);

		foreach (Unit u in unitsInBattle) {
			if (u.clickable && u.mouseOver) {
				return u.gridPosition;
			}
		};

		// if you can't find a valid Unit, get the Grid location
		return grid.GetMouseToGridPos();
	}

	public override HashSet<Vector3Int> GetObstacles() {		
		return Battle.active.grid.CurrentOccupantPositionsExcepting<PlayerUnit>();
	}
}