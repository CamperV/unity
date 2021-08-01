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

	public IEnumerable<Vector3Int> Positions { get => GetPositions(); }

	void Awake() {
		baseTilemap = GetComponent<Tilemap>();
		overlayTilemap = GetComponentsInChildren<Tilemap>()[0];

		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
		overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();
	}

	private IEnumerable<Vector3Int> GetPositions() {
		foreach (var pos in baseTilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (baseTilemap.HasTile(v)) {
				Debug.Log($"found pos {v}");
				yield return v;
			}
		}
	}

	public TacticsTile GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos) as TacticsTile;
	}

	public List<Vector3Int> GetSpawnLocations() {
		// get all set tiles in the tilemap
		// determine their types, and return the appropriate ones
		return new List<Vector3Int>();
	}
}
