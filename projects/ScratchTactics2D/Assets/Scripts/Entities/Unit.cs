using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	// to be defined at a lower level
	public abstract int movementRange { get; }
	public abstract int attackReach { get; }

	// cache the movement range for easier lookup later
	public MoveRange moveRange;
	public AttackRange attackRange;

	protected Dictionary<string, bool> optionAvailability = new Dictionary<string, bool>() {
		["Move"]	= true,
		["Attack"]	= true
	};

	public bool OptionActive(string optionToCheck) {
		if (!optionAvailability.ContainsKey(optionToCheck)) {
			throw new System.Exception($"{optionToCheck} not a valid option to check"); 
		}
		return optionAvailability[optionToCheck];
	}

	public void SetOption(string option, bool setting) {
		if (!optionAvailability.ContainsKey(option)) {
			throw new System.Exception($"{option} not a valid option to set"); 
		}
		optionAvailability[option] = setting;
	}

	public bool AnyOptionActive() {
		foreach (var k in optionAvailability.Keys.ToList()) {
			if (optionAvailability[k]) return true;
		}
		return false;
	}

	// valid Unit Actions:
	// OnSelect
	// ShowMovementRange
	// TraverseTo
	// Attack
	// Wait
	// Other
	public void OnStartTurn() {
		foreach (var k in optionAvailability.Keys.ToList()) {
			optionAvailability[k] = true;
		}
		spriteRenderer.color = Color.white;
		//
		UpdateThreatRange();
	}

	public void OnEndTurn() {
		foreach (var k in optionAvailability.Keys.ToList()) {
			optionAvailability[k] = false;
		}

		StartCoroutine(ExecuteAfterMoving(() => {
			spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		})); 
	}

	public void UpdateThreatRange() {
		HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(GameManager.inst.tacticsManager.GetActiveGrid().GetAllTilePos());

		moveRange = MoveRange.MoveRangeFrom(gridPosition, tiles, range: movementRange);
		attackRange = AttackRange.AttackRangeFrom(moveRange, tiles, range: attackReach);
	}

	public void OnSelect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// display movement range
		spriteRenderer.color = Utils.selectColorRed;
		
		UpdateThreatRange();
		attackRange?.Display(grid);
		moveRange?.Display(grid);
	}

	public void OnDeselect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		//
		spriteRenderer.color = Color.white;
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);
	}

	public void TraverseTo(Vector3Int target, MovingObjectPath fieldPath = null) {
		GameGrid grid = GameManager.inst.tacticsManager.GetActiveGrid();
		if (fieldPath == null) {
			fieldPath = MovingObjectPath.GetPathFromField(target, moveRange);
		}

		// movement animation
		StartCoroutine(SmoothMovementPath(fieldPath, grid));

		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(target, this);
		gridPosition = target;
	}
}
