using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	// to be defined at a lower level
	public abstract int movementRange { get; }

	// cache the movement range for easier lookup later
	private FlowField _range;
	public FlowField range {
		get {
			if (_range == null) {
				_range = GetMovementRange();
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
		ShowMovementRange();
	}

	public void OnDeselect() {
		spriteRenderer.color = Color.white;
		//
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		foreach (Vector3Int tilePos in range.field.Keys) {
			grid.ResetSelectionAt(tilePos);
		}
	}

	public void ShowMovementRange() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		foreach (Vector3Int tilePos in range.field.Keys) {
			grid.SelectAt(tilePos);
		}
	}

	private FlowField GetMovementRange() {
		HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(GameManager.inst.tacticsManager.GetActiveGrid().GetAllTilePos());
		return FlowField.FlowFieldFrom(gridPosition, tiles, range: movementRange);
	}
}
