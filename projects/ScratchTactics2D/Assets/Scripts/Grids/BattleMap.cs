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
		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();

		overlayTilemap = GetComponentsInChildren<Tilemap>()[1];
		overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();
	}

	public TacticsTile GetBattleMapTile(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos) as TacticsTile;
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

	public void RotateBattleMap(Vector3Int orientation) {
		// this function actually only rotates to the left - we don't need to fully transform these maps
		// only transform the orientation and spawn points
		if (orientation == Vector3Int.right || orientation == Vector3Int.left) {
			RotateTilemap(baseTilemap, v => new Vector3Int(-v.y, v.x, v.z));
			RotateTilemap(overlayTilemap, v => new Vector3Int(-v.y, v.x, v.z));
		}
		
		// no need to rotate, this is how the BattleMap is created originally
		// if (orientation == Vector3Int.down || orientation == Vector3Int.up) {
		// 	return;
		// }
	}

	// TODO: doesn't need to be here, re-locate
	public static void RotateTilemap(Tilemap tilemap, Func<Vector3Int, Vector3Int> Transformer) {
		Dictionary<Vector3Int, Vector3Int> rotated = new Dictionary<Vector3Int, Vector3Int>();
		Dictionary<Vector3Int, TacticsTile> wasTile = new Dictionary<Vector3Int, TacticsTile>();

		foreach (Vector3Int tilePos in GetPositions(tilemap)) {
			wasTile[tilePos] = (tilemap.GetTile(tilePos) as TacticsTile); // this can't be null by definition
			rotated[tilePos] = Transformer(tilePos);
			tilemap.SetTile(tilePos, null);
		}
		foreach (Vector3Int tilePos in wasTile.Keys) {
			tilemap.SetTile(rotated[tilePos], wasTile[tilePos]);
		}

		tilemap.CompressBounds();
		tilemap.RefreshAllTiles();
	}

	private IEnumerable<Vector3Int> GetPositionsOfType<T>(Tilemap tilemap) where T : TacticsTile {
		// get all set tiles in the tilemap
		// determine their types, and return the appropriate ones
		foreach (var tilePos in GetPositions(tilemap)) {
			var tile = tilemap.GetTile<T>(tilePos);
			if (tile != null) yield return tilePos;
		}
	}

	private static IEnumerable<Vector3Int> GetPositions(Tilemap tilemap) {
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (tilemap.HasTile(v)) yield return v;
		}
	}
}
