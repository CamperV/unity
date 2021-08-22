using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BattlePath : Path
{
	private OverlayTile pathEndTile;
	private OverlayTile pathTile;

	public BattlePath() {
		pathEndTile = PathOverlayIsoTile.GetTileWithSprite(1);
		pathTile 	= (ScriptableObject.CreateInstance<PathOverlayIsoTile>() as PathOverlayIsoTile);
	}

	public void Show() {
		foreach (Vector3Int tilePos in Unwind()) {
			if (tilePos == end) break; // skip end tile for debug
			Battle.active.grid.OverlayAt(tilePos, pathTile);
		}
		GameManager.inst.overworld.OverlayAt(end, pathEndTile);
	}

	public void UnShow() {
		// slice 1 will clip the start position out
		foreach (Vector3Int tilePos in Unwind()) {
			Battle.active.grid.ResetOverlayAt(tilePos);
		}
	}
}
