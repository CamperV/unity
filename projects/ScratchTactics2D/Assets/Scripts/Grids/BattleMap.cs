using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class BattleMap : MonoBehaviour
{
	private Tilemap baseTilemap;
	private Tilemap overlayTilemap;

	public IEnumerable<Vector3Int> Positions { get => GetPositions(baseTilemap); }

	private Vector3Int _playerEnemyOrientation;
	public Vector3Int playerEnemyOrientation {
		get => _playerEnemyOrientation;
		set {
			_playerEnemyOrientation = value;
			Debug.Log($"Set playerEnemyOrientation: {value}");
		}
	}

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

	public void RotateBattleMap(Vector3Int orientation) {
		// this function actually only rotates to the left - we don't need to fully transform these maps
		// only transform the orientation and spawn points
		if (orientation == Vector3Int.right || orientation == Vector3Int.left) {
			RotateTilemap(baseTilemap, BattleCamera.RotateLeft);
			RotateTilemap(overlayTilemap, BattleCamera.RotateLeft);
		}
		
		// // no need to rotate, this is how the BattleMap is created originally
		// if (orientation == Vector3Int.down || orientation == Vector3Int.up)
	}

	public void RotateDocker(Vector3Int orientation) {
		if (orientation == Vector3Int.up) {
			RotateTilemap(baseTilemap, BattleCamera.RotateLeft);
			RotateTilemap(overlayTilemap, BattleCamera.RotateLeft);
		}
		if (orientation == Vector3Int.down) {
			RotateTilemap(baseTilemap, BattleCamera.RotateRight);
			RotateTilemap(overlayTilemap, BattleCamera.RotateRight);
		}
		if (orientation == Vector3Int.right) {
			RotateTilemap(baseTilemap, BattleCamera.RotateRight);
			RotateTilemap(overlayTilemap, BattleCamera.RotateRight);
			RotateTilemap(baseTilemap, BattleCamera.RotateRight);
			RotateTilemap(overlayTilemap, BattleCamera.RotateRight);
		}
		// if (orientation == Vector3Int.left) {
		// 	// don't rotate, this is default
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

	public static IEnumerable<Vector3Int> GetPositionsOfType<T>(Tilemap tilemap) where T : TacticsTile {
		// get all set tiles in the tilemap
		// determine their types, and return the appropriate ones
		foreach (var tilePos in GetPositions(tilemap)) {
			var tile = tilemap.GetTile<T>(tilePos);
			if (tile != null) yield return tilePos;
		}
	}

	protected static IEnumerable<Vector3Int> GetPositions(Tilemap tilemap) {
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
		}
		return new Zone(positions);
	}

	// we make the specification here because your docking point depends on the original orientation of the Player:Enemy
	public List<Vector3Int> GetDockingPointsFromJoiningOrientation(Vector3Int joiningOrientation) {
		List<Vector3Int> positions = new List<Vector3Int>();

		// | P | E |
		// can never return DockingMarkerTT_1
		if (playerEnemyOrientation == Vector3Int.left) {
			if (joiningOrientation == Vector3Int.right) {
				return GetPositionsOfType<DockingMarkerTacticsTile_0>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.up) {
				return GetPositionsOfType<DockingMarkerTacticsTile_2>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.down) {
				return GetPositionsOfType<DockingMarkerTacticsTile_3>(overlayTilemap).ToList();
			}
		}

		// | E | P |
		// can never return DockingMarkerTT_0
		if (playerEnemyOrientation == Vector3Int.right) {
			if (joiningOrientation == Vector3Int.left) {
				return GetPositionsOfType<DockingMarkerTacticsTile_1>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.up) {
				return GetPositionsOfType<DockingMarkerTacticsTile_2>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.down) {
				return GetPositionsOfType<DockingMarkerTacticsTile_3>(overlayTilemap).ToList();
			}
		}

		// | E |
		//  ---
		// | P |
		// can never return DockingMarkerTT_1
		if (playerEnemyOrientation == Vector3Int.down) {
			if (joiningOrientation == Vector3Int.left) {
				return GetPositionsOfType<DockingMarkerTacticsTile_2>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.up) {
				return GetPositionsOfType<DockingMarkerTacticsTile_0>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.right) {
				return GetPositionsOfType<DockingMarkerTacticsTile_3>(overlayTilemap).ToList();
			}
		}

		// | P |
		//  ---
		// | E |
		// can never return DockingMarkerTT_0
		if (playerEnemyOrientation == Vector3Int.up) {
			if (joiningOrientation == Vector3Int.left) {
				return GetPositionsOfType<DockingMarkerTacticsTile_2>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.down) {
				return GetPositionsOfType<DockingMarkerTacticsTile_1>(overlayTilemap).ToList();
			} else if (joiningOrientation == Vector3Int.right) {
				return GetPositionsOfType<DockingMarkerTacticsTile_3>(overlayTilemap).ToList();
			}
		}
		return null;
	}

	public List<Vector3Int> GetAllDockingPoints() {
		List<Vector3Int> positions = new List<Vector3Int>();
		GetPositionsOfType<DockingMarkerTacticsTile_0>(overlayTilemap).ToList().ForEach(v => positions.Add(v));
		GetPositionsOfType<DockingMarkerTacticsTile_1>(overlayTilemap).ToList().ForEach(v => positions.Add(v));
		GetPositionsOfType<DockingMarkerTacticsTile_2>(overlayTilemap).ToList().ForEach(v => positions.Add(v));
		GetPositionsOfType<DockingMarkerTacticsTile_3>(overlayTilemap).ToList().ForEach(v => positions.Add(v));
		return positions;
	}

	public Zone GetDockerSpawnZone(Vector3Int offset) {
		List<Vector3Int> positions = new List<Vector3Int>();
		GetPositionsOfType<SpawnMarkerTacticsTile_0>(overlayTilemap).ToList().ForEach(v => positions.Add(v + offset));
		return new Zone(positions);
	}

	// always animate this BEFORE setting the baseTilemap
    public float AnimateDocking(Vector3Int dockingOffset, List<Unit> spawnedUnits, Vector3Int orientation) {
		float motionTime = 0.25f;
        float delayTime = 0.0f;
        
		Dictionary<Vector3Int, TacticsTile> finalPositions = new Dictionary<Vector3Int, TacticsTile>();
		var _finalPositions = GameGrid.GetTilemapDict<TacticsTile>(baseTilemap);
		foreach (Vector3Int _final in _finalPositions.Keys) {
			finalPositions[_final + dockingOffset] = _finalPositions[_final];
		}

		// I hate that this code found its way all the way down here, but:
		//  set the transparency for a set time for all affected tiles and units
		// for all the final positions, set their transparency in the true Battle.active
		// for all the spawnedUnits, set transparency for totalTime
        float totalTime = (delayTime * finalPositions.Keys.Count) + motionTime;
		StartCoroutine( _InvisibleFor(totalTime, finalPositions, spawnedUnits) );
        
		// create the MovingSprite facsimilies and send them on their way
        // animate all the tiles first
		int initialDistance = 5;

    	foreach (Vector3Int _to in finalPositions.Keys) {
            TacticsTile tt = finalPositions[_to];

			Vector3Int _from = _to + new Vector3Int(-initialDistance*orientation.y, initialDistance*orientation.x, 0);
			Vector3 from = Battle.active.grid.Grid2RealPos(_from);
			Vector3 to = Battle.active.grid.Grid2RealPos(_to);
			 
			MovingSprite anim = MovingSprite.ConstructWith(from, tt.sprite, "Tactics Entities");
            anim.SendToAndDestroyFadeUp(to, motionTime);
		}

        // for units and such, we need them to be on top of their own tile, but not obscuring others
		foreach (Unit u in spawnedUnits) {
			Vector3 _sortingOffset =  new Vector3(0, 0, 2f);

			Vector3Int _to = u.gridPosition;
			Vector3Int _from = _to + new Vector3Int(-initialDistance*orientation.y, -initialDistance*orientation.x, 0);
			Vector3 from = Battle.active.grid.Grid2RealPos(_from) + _sortingOffset;
			Vector3 to = Battle.active.grid.Grid2RealPos(_to) + _sortingOffset;

        	MovingSprite anim = MovingSprite.ConstructWith(from, u.GetComponent<SpriteRenderer>().sprite, "Tactics Entities");
			anim.transform.localScale = u.transform.localScale;
            anim.SendToAndDestroyFadeUp(to, motionTime);
		}

		return totalTime;
    }

	// this is hardcoded to the active Battle
	private static IEnumerator _InvisibleFor(float fixedTime, Dictionary<Vector3Int, TacticsTile> tiles, List<Unit> units) {
		Color _invis = Color.white.WithAlpha(0f);

		// this is for re-coloring the tileMaps
		Dictionary<Vector3Int, Color> tileColors = new Dictionary<Vector3Int, Color>();
		foreach (Vector3Int v in tiles.Keys) {
			tileColors[v] = Battle.active.grid.GetTint(Battle.active.grid.baseTilemap, v);
			Battle.active.grid.TintTile(Battle.active.grid.baseTilemap, v, _invis);
		}

		Dictionary<Unit, Color> uColors = new Dictionary<Unit, Color>();
		foreach (Unit u in units) {
			uColors[u] = u.GetComponent<SpriteRenderer>().color;
			u.SetTransparency(0f);
		}

		// wait quite procedurally
		yield return new WaitForSeconds(fixedTime);

		// now re-color with saved colors
		foreach (Unit u in uColors.Keys) {
			u.SetTransparency(uColors[u].a);
		}
		foreach (Vector3Int v in tileColors.Keys) {
			Battle.active.grid.TintTile(Battle.active.grid.baseTilemap, v, tileColors[v]);
		}
	}
}
