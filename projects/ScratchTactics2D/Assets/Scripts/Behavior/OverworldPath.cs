using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OverworldPath : Path
{
	private OverlayTile pathEndTile;
	private OverlayTile pathEndTileInteract;
	private OverlayTile pathTile;

	private bool _interactFlag;
	public bool interactFlag {
		get => _interactFlag;
		set {
			_interactFlag = value;
			pathEndTile = (_interactFlag) ? pathEndTileInteract : pathEndTile;
		}
	}

	public OverworldPath() {
		pathEndTile 		= (ScriptableObject.CreateInstance<EndpointOverlayTile>() as EndpointOverlayTile);
		pathEndTileInteract = (ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile);
		pathTile 			= (ScriptableObject.CreateInstance<PathOverlayTile>() as PathOverlayTile);
	}

	public void Show() {
		foreach (Vector3Int tilePos in Unwind()) {
			if (tilePos == end) break; // skip end tile for debug
			GameManager.inst.overworld.OverlayAt(tilePos, pathTile);
		}
		GameManager.inst.overworld.OverlayAt(end, pathEndTile);
	}

	public void UnShow() {
		// slice 1 will clip the start position out
		foreach (Vector3Int tilePos in Unwind()) {
			GameManager.inst.overworld.ResetOverlayAt(tilePos);
		}
	}

	// will UnShow the initial input
	public void UnShowUntil(Vector3Int pos) {
		GameManager.inst.overworld.ResetOverlayAt(pos);
		if (pos != start) {
			var m = GetPrevious(pos);
			Debug.Log($"unshowing {m} ({start})");
			UnShowUntil( m );
		}
	}
}
