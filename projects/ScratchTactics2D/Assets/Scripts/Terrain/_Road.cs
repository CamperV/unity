using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class _Road
{
	public Vector3Int start;
	public Vector3Int end;

	private Path path;

	public _Road(Vector3Int startPos, Vector3Int endPos) {
		start = startPos;
		end = endPos;
		path = new ArmyPathfinder().BFS(startPos, endPos);
	}

	public IEnumerable<Vector3Int> Unwind(int slice = 0) {
		return path.Unwind();
	}

	public void Apply(Overworld grid) {

		// for every tile that should be a road
		// always slice away the first startPos, because it's connected to something
		foreach(Vector3Int roadPos in Unwind()) {

			// create Pattern of Road positions center
			TerrainPattern3x3 pattern = new TerrainPattern3x3();
			foreach(Vector3Int neighbor in grid.GetNeighbors(roadPos)) {

				// if the neighboring Terrain is also of type Road
				Type terrainAt = grid.TerrainAt(neighbor).GetType();
				if (terrainAt == this.GetType() || terrainAt == typeof(Village)) {
					pattern.Add( neighbor - roadPos );	
				}
			}

			// now set based on the current Type
			Type tileType = grid.TypeAt(roadPos);
			WorldTile roadTile = null;
			
			if (tileType == typeof(GrassWorldTile)) {
				roadTile = pattern.GetPatternTile<RoadWorldTile>();
			}
			else if (tileType == typeof(ForestWorldTile)) {
				roadTile = pattern.GetPatternTile<ForestRoadWorldTile>();
			}				
			else if (tileType == typeof(WaterWorldTile)) {
				roadTile = pattern.GetPatternTile<WaterRoadWorldTile>();
			}
			else if (tileType == typeof(DeepWaterWorldTile)) {
				roadTile = pattern.GetPatternTile<WaterRoadWorldTile>();
			}
			else if (tileType == typeof(MountainWorldTile)) {
				roadTile = pattern.GetPatternTile<MountainRoadWorldTile>();
			}
			else if (tileType == typeof(VillageWorldTile)) {
				roadTile = pattern.GetPatternTile<VillageRoadWorldTile>();
			}
			else {
				roadTile = pattern.GetPatternTile<RoadWorldTile>();
			}

			grid.SetAppropriateTile(roadPos, roadTile);				
		}
	}
}