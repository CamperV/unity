using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BattleMap : MonoBehaviour
{
	private Tilemap baseTilemap;
	private Tilemap overlayTilemap;

	public IEnumerable<Vector3Int> Positions { get => GetPositions(baseTilemap); }

	void Awake() {
		baseTilemap = GetComponentsInChildren<Tilemap>()[0];
		overlayTilemap = GetComponentsInChildren<Tilemap>()[1];

		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
		overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();
	}

	private IEnumerable<Vector3Int> GetPositions(Tilemap tilemap) {
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (tilemap.HasTile(v)) yield return v;
		}
	}

	// orientation here refers to the player
	// i.e., an orientation of Vector3Int.down implies the following: (Enemy v Player)
	//		___
	//	   | E |
	//	    ---
	//	   | P |
	//      ---
	public Zone GetSpawnZoneFromOrientation(Vector3Int orientation) {
		List<Vector3Int> positions = new List<Vector3Int>();

		// get either spawn zone 1 (near) or spawn zone 2 (far)
		// transforming E/W is done outside this function
		if (orientation == Vector3Int.down || orientation == Vector3Int.left) {
			positions = GetPositionsOfType<SpawnMarkerTacticsTile_0>(overlayTilemap).ToList();
		} else if (orientation == Vector3Int.up || orientation == Vector3Int.right) {
			positions = GetPositionsOfType<SpawnMarkerTacticsTile_1>(overlayTilemap).ToList();
		} else {
			Debug.Log($"Invalid orientation of player/enemy battle initiation");
			Debug.Assert(false);
		}
		return new Zone(positions);
	}

	public TacticsTile GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos) as TacticsTile;
	}

	private IEnumerable<Vector3Int> GetPositionsOfType<T>(Tilemap tilemap) where T : TacticsTile {
		// get all set tiles in the tilemap
		// determine their types, and return the appropriate ones
		foreach (var tilePos in GetPositions(tilemap)) {
			var tile = tilemap.GetTile<T>(tilePos);
			if (tile != null) yield return tilePos;
		}
	}
}
