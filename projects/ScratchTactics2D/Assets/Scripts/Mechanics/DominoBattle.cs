using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Extensions;
using Random = UnityEngine.Random;

public static class DominoBattle
{
    // public void CreateDominoTileMap(Vector3Int offset, Terrain originTerrain) {
	// 	var newOrigin = baseTilemap.origin + offset;

	// 	for (int x = 0; x < originTerrain.battleGridSize.x; x++) {
	// 		for (int y = 0; y < originTerrain.battleGridSize.y; y++) {
	// 			Vector3Int pos = newOrigin + new Vector3Int(x, y, Random.Range(0, 2));
	// 			tacticsTileGrid[pos] = originTerrain.tacticsTile;
	// 			surface[new Vector2Int(pos.x, pos.y)] = pos;
	// 		}
	// 	}
	// }

	// public void ApplyTileGrid(bool noCompress = false) {
	// 	Debug.Assert(tacticsTileGrid.Count != 0);
		
	// 	foreach(var pair in tacticsTileGrid.OrderBy(k => k.Key.x)) {
	// 		baseTilemap.SetTile(pair.Key, pair.Value);
	// 	}

	// 	if (noCompress) return;
	// 	baseTilemap.CompressBounds();
	// 	baseTilemap.RefreshAllTiles();

	// 	// after refreshing all tiles, set rudimentary shading
	// 	float tintScale = 0.20f;
	// 	int maxZ = tacticsTileGrid.Keys.Max(it => it.z);
	// 	foreach(var pair in tacticsTileGrid) {
	// 		// tint the lower tiles by a step value, to create some shade
	// 		// and fill out the tiles all the way to the bottom (of the screen, so like 10 for now)
	// 		for (int i = pair.Key.z; i > -3; i--) {
	// 			Vector3Int lower = new Vector3Int(pair.Key.x, pair.Key.y, i);
	// 			baseTilemap.SetTile(lower, pair.Value);

	// 			baseTilemap.SetTileFlags(lower, TileFlags.None);
	// 			float tint = 1.0f - (tintScale * (maxZ - i));
	// 			//float a = (i > -3) ? 1.0f : 1.0f - ( (tintScale/2.0f) * (-3 - i));
	// 			float a = 1.0f;
	// 			baseTilemap.SetColor(lower, new Color(tint, tint, tint, a));
	// 		}
	// 	}
	// }

	// // we can only start a Battle with two participants
	// // however, others can join(?)
	// public void CreateDominoTacticsGrid(Terrain playerTerrain, Terrain otherTerrain) {	
	// 	// determine orientations
	// 	Dictionary<Vector3Int, List<Vector3Int>> orientationDict = new Dictionary<Vector3Int, List<Vector3Int>>() {
	// 		[Vector3Int.up] = new List<Vector3Int>() {
	// 			Vector3Int.zero,
	// 			new Vector3Int(playerTerrain.battleGridSize.x, 0, 0)
	// 		}, 
	// 		[Vector3Int.right] = new List<Vector3Int>() {
	// 			new Vector3Int(0, otherTerrain.battleGridSize.y, 0),
	// 			Vector3Int.zero
	// 		}, 
	// 		[Vector3Int.down] = new List<Vector3Int>() {
	// 			new Vector3Int(otherTerrain.battleGridSize.x, 0, 0),
	// 			Vector3Int.zero
	// 		}, 
	// 		[Vector3Int.left] = new List<Vector3Int>() {
	// 			Vector3Int.zero,
	// 			new Vector3Int(0, playerTerrain.battleGridSize.y, 0)
	// 		}
	// 	};
	// 	var offsets = orientationDict[(other.gridPosition - player.gridPosition)];

	// 	// store for later
	// 	playerGridOffset = offsets[0];
		
	// 	// setup up each side	
	// 	// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
	// 	grid.CreateDominoTileMap(offsets[0], playerTerrain);
	// 	grid.CreateDominoTileMap(offsets[1], otherTerrain);
		
	// 	// after all battle participants have generated their TileMaps, apply the contents of the tacticsTileGrid to the baseTilemap
	// 	// then compress the bounds afterwards
	// 	grid.ApplyTileGrid();
		
	// 	// determine correct centering factor
	// 	// move to center after the tilemap has been filled
	// 	Vector3 gridCenter = grid.GetGridCenterReal();
	// 	Vector3 offsetPos = transform.position - (gridCenter - transform.position);
		
	// 	grid.transform.position = offsetPos;
	// }
	
	// public void SpawnAllUnitsDomino() {
	// 	// number of spawnZones is equal to the number of worldParticpants (2)
	// 	Pair<SpawnZone, SpawnZone> spawnZones = GetSpawnZonesDomino();
		
	// 	// do spawn-y things and add them to the activeUnit registry
	// 	// in the future, assign them to a Director (either player control or AI)
	// 	var playerSpawnPositions = spawnZones.first.GetPositions().RandomSelections<Vector3Int>(player.numUnits);

	// 	// the player will maintain a barracks of units
	// 	// the player has reference to each prefab needed, so we instantiate a prefab here
	// 	// then apply the actual relevant stats
	// 	foreach (UnitState unitState in player.GetUnits()) {
	// 		var uPrefab = player.LoadUnitByTag("Units/" + unitState.unitTag);
	// 		PlayerUnit unit = TacticsEntityBase.Spawn<Unit>(uPrefab, playerSpawnPositions.PopAt(0), grid) as PlayerUnit;
	// 		//
	// 		unit.ApplyState(unitState);
	// 		GetController(player).Register(unit);
	// 	}

	// 	// LoadUnitsByTag will look up if an appropriate prefab has already been loaded from the Resources folder
	// 	// if it has, it will instantiate it. If not, it will load first
	// 	var otherSpawnPositions = spawnZones.second.GetPositions().RandomSelections<Vector3Int>(other.numUnits);

	// 	foreach (UnitState unitState in other.GetUnits()) {
	// 		var uPrefab = other.LoadUnitByTag("Units/" + unitState.unitTag);
	// 		EnemyUnit unit = TacticsEntityBase.Spawn<Unit>(uPrefab, otherSpawnPositions.PopAt(0), grid) as EnemyUnit;
	// 		//
	// 		unit.ApplyState(unitState);
	// 		GetController(other).Register(unit);
	// 	}
	// }

	// public void SpawnObstaclesDomino() {
	// 	Zone spawnZone = Zone.WithinGrid(grid, Vector3Int.zero, grid.GetDimensions() - Vector3Int.one);
	// 	var spawnPositions = spawnZone.GetPositions().RandomSelections<Vector3Int>(Random.Range(5, 10));

	// 	// TODO: get Obstacle from Tile type
	// 	foreach (Vector3Int pos in spawnPositions) {
	// 		Obstacle oPrefab = Resources.Load<Obstacle>("ObstacleTree");
	// 		Obstacle obs = TacticsEntityBase.Spawn<Obstacle>(oPrefab, pos, grid);
	// 		obs.transform.SetParent(this.transform);
	// 	}
	// }

	// private Pair<SpawnZone, SpawnZone> GetSpawnZonesDomino() {
	// 	//
	// 	// IMPORTANT: the north/suth/east/west scaling (0.4 currently) is linked to the size of the spawn zones
	// 	// if you're going to do this programmatically in the future, probably just switch to vector rotation

	// 	float wtfAngle = 63.565f;
	// 	Vector3 scaledRadius = 0.85f * grid.GetCellRadius2D();
	// 	Vector3 northeastVec = Quaternion.AngleAxis( wtfAngle, 			new Vector3(0, 0, -1)) * scaledRadius;
	// 	Vector3 northwestVec = Quaternion.AngleAxis(-wtfAngle, 			new Vector3(0, 0, -1)) * scaledRadius;
	// 	Vector3 southeastVec = Quaternion.AngleAxis( (180f - wtfAngle), new Vector3(0, 0, -1)) * scaledRadius;
	// 	Vector3 southwestVec = Quaternion.AngleAxis(-(180f - wtfAngle), new Vector3(0, 0, -1)) * scaledRadius;
	// 	Vector3 northeast = grid.GetGridCenterReal() + northeastVec;
	// 	Vector3 northwest = grid.GetGridCenterReal() + northwestVec;
	// 	Vector3 southeast = grid.GetGridCenterReal() + southeastVec;
	// 	Vector3 southwest = grid.GetGridCenterReal() + southwestVec;

	// 	//Debug.DrawLine(grid.GetGridCenterReal(), northeast, Color.red, 1000.0f, false);
	// 	//Debug.DrawLine(grid.GetGridCenterReal(), northwest, Color.blue, 1000.0f, false);
	// 	//Debug.DrawLine(grid.GetGridCenterReal(), southwest, Color.green, 1000.0f, false);
	// 	//Debug.DrawLine(grid.GetGridCenterReal(), southeast, Color.yellow, 1000.0f, false);

	// 	// literally just a char to determine if it's an N-S orientation or an E-W orientation
	// 	// the slashes just help me remember
	// 	string orientation = "//";
	// 	Vector3 playerAnchor = Vector3.zero;
	// 	Vector3 otherAnchor = Vector3.zero;
	// 	switch (other.gridPosition - player.gridPosition) {
	// 		case Vector3Int v when v.Equals(Vector3Int.up):
	// 			playerAnchor = southwest;
	// 			otherAnchor = northeast;
	// 			//
	// 			orientation = "//";
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.right):
	// 			playerAnchor = northwest;
	// 			otherAnchor = southeast;
	// 			//
	// 			orientation = "\\";
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.down):
	// 			otherAnchor = southwest;
	// 			playerAnchor = northeast;
	// 			//
	// 			orientation = "//";
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.left):
	// 			otherAnchor = northwest;
	// 			playerAnchor = southeast;
	// 			//
	// 			orientation = "\\";
	// 			break;
	// 	}

	// 	// create the zone to spawn units into
	// 	// randomly select which starting positions happen, for now
	// 	const int width = 6;
	// 	const int height = 4;
	// 	const int depth = 2;
	// 	int xSize = (orientation == "//") ? height : width;
	// 	int ySize = (orientation == "//") ? width : height;
	// 	int zSize = depth;
	// 	return new Pair<SpawnZone, SpawnZone>(
	// 		new SpawnZone(playerAnchor, xSize-1, ySize-1, zSize, grid),
	// 		new SpawnZone(otherAnchor, xSize-1, ySize-1, zSize, grid)
	// 	);
	// }

	// public void AddParticipantDomino(Army joiningEntity, Terrain joiningTerrain) {
	// 	(joiningEntity as EnemyArmy).state = Enum.EnemyArmyState.inBattle;
	// 	allOther.Add(joiningEntity);

	// 	// add to grid and reposition
	// 	Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(player.gridPosition);
	// 	Dictionary<Vector3Int, Vector3Int> orientationDict = new Dictionary<Vector3Int, Vector3Int>() {
	// 		[Vector3Int.up]    = playerGridOffset + new Vector3Int(playerTerrain.battleGridSize.x, 0, 0),
	// 		[Vector3Int.right] = playerGridOffset - new Vector3Int(0, joiningTerrain.battleGridSize.y, 0),
	// 		[Vector3Int.down]  = playerGridOffset - new Vector3Int(joiningTerrain.battleGridSize.x, 0, 0), 
	// 		[Vector3Int.left]  = playerGridOffset + new Vector3Int(0, playerTerrain.battleGridSize.y, 0)
	// 	};
	// 	var offset = orientationDict[(joiningEntity.gridPosition - player.gridPosition)];
		
	// 	// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
	// 	Debug.Log($"Creating a new battleground with offset {offset}");
	// 	grid.CreateDominoTileMap(offset, joiningTerrain);
	// 	grid.ApplyTileGrid(noCompress: false);
	// 	//
	// 	Vector3 gridCenter = grid.GetGridCenterReal();
	// 	Vector3 offsetPos = transform.position - (gridCenter - transform.position);
	// 	transform.position = offsetPos;

	// 	// spawn new units
	// 	// register new units to existing controller
	// 	// but... keep tabs, because we need to use this to kill OverworldEntities
	// 	// TODO: kill overworld entities better, I guess
	// 	Controller existingEnemyArmyController = activeControllers[other];
	// 	activeControllers[joiningEntity] = existingEnemyArmyController;
	// 	//

	// 	//
	// 	// get SpawnZone
	// 	//
	// 	Vector3Int A = Vector3Int.zero;
	// 	Vector3Int B = Vector3Int.zero;

	// 	// helpers
	// 	Vector3Int min  = offset;
	// 	Vector3Int xDim = new Vector3Int(joiningTerrain.battleGridSize.x-1, 0, 0);
	// 	Vector3Int yDim = new Vector3Int(0, joiningTerrain.battleGridSize.y-1, 0); 
	// 	Vector3Int max  = min + xDim + yDim;
	// 	switch (joiningEntity.gridPosition - player.gridPosition) {
	// 		case Vector3Int v when v.Equals(Vector3Int.up):
	// 			A = max;
	// 			B = max - yDim - xDim.DivBy(4);
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.right):
	// 			A = min;
	// 			B = min + xDim + yDim.DivBy(4);
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.down):
	// 			A = min;
	// 			B = min + yDim + xDim.DivBy(4);
	// 			break;
	// 		case Vector3Int v when v.Equals(Vector3Int.left):
	// 			A = max;
	// 			B = max - xDim - yDim.DivBy(4);
	// 			break;
	// 	}
	// 	Zone spawnZone = new Zone(A, B);

	// 	//
	// 	// spawn those units
	// 	// register them 
	// 	//
	// 	var spawnPositions = spawnZone.GetPositions().RandomSelections<Vector3Int>(joiningEntity.numUnits);
	// 	foreach (UnitState unitState in joiningEntity.GetUnits()) {
	// 		var uPrefab = joiningEntity.LoadUnitByTag("Units/" + unitState.unitTag);
	// 		Unit unit = TacticsEntityBase.Spawn<Unit>(uPrefab, spawnPositions.PopAt(0), grid);
	// 		//
	// 		unit.ApplyState(unitState);
	// 		existingEnemyArmyController.Register(unit);
	// 	}	
	// }
}
