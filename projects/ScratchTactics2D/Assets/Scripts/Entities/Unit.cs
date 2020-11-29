using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	// to be defined at a lower level
	public abstract int movementRange { get; }

	// cache the movement range for easier lookup later
	private MoveRange _range;
	public MoveRange range {
		get {
			if (_range == null) {
				HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(GameManager.inst.tacticsManager.GetActiveGrid().GetAllTilePos());
				_range = MoveRange.MoveRangeFrom(gridPosition, tiles, range: movementRange);
			}
			return _range;
		}
	}

	// valid Unit Actions:
	// OnSelect
	// ShowMovementRange
	// Move
	// Attack
	// Wait
	// Other
	public void OnSelect() {
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// display movement range
		spriteRenderer.color = Utils.selectColorRed;
		range.Display(GameManager.inst.tacticsManager.GetActiveGrid());
	}

	public void OnDeselect() {
		spriteRenderer.color = Color.white;
		range.ClearDisplay(GameManager.inst.tacticsManager.GetActiveGrid());
	}

	public void TraverseTo(Vector3Int target, MovingObjectPath fieldPath = null) {
		GameGrid grid = GameManager.inst.tacticsManager.GetActiveGrid();

		if (fieldPath == null) {
			fieldPath = MovingObjectPath.GetPathFromField(target, range);
		}

		// movement animation
		StartCoroutine(SmoothMovementPath(fieldPath, grid));

		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(target, this);
		gridPosition = target;

		_range = null;	// this resets so that MoveRange will recalculate when called
	}
}
