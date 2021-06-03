﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class RandomTerrainGenerator : TerrainGenerator
{
	// tileOptions determine probability order as well
	// so when Overworld is generated, it will check if the tile is:
	//  grass first, then dirt, then water, then mountain
	public override void GenerateMap() {
		map = new TileEnum[mapDimension.x, mapDimension.y];
		
		// randomly select all other tiles
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				// this determines which tile is chosen
				var rng = Random.Range(1, 101); // exclusive
				
				int selection;
				int probCounter = 0;
				for(selection = 0; selection < tileOptions.Length; selection++) {
					probCounter += tileOptions[selection].probability;
					if (rng <= probCounter) break;
				}
				Debug.Assert(probCounter <= 100);
				map[i, j] = (TileEnum)selection;
			}
		}
	}

	protected override void Postprocessing() {
		LinkMountainRanges();
		CreateForests(2, 5);
		CreateLakes(5, 10);

		// player-affecting tiles
		PlaceVillages(10);
	}

	private void LinkMountainRanges() {
		// choose some %age of mountains to link
		List<Vector3Int> posList = PositionsOfType(TileEnum.mountain);
		foreach (var mnt in posList) { GameManager.inst.overworld.SetTerrainAt(mnt, new Mountain(mnt)); }
		List<Vector3Int> toLink = posList.RandomSelections<Vector3Int>((int)(posList.Count/2));
		
		// for each linking mountain, find the closest next mountain, and link to it
		foreach(Vector3Int startMountain in toLink) {
			Vector3Int endMountain = ClosestOfType(startMountain, TileEnum.mountain);
			
			// create paths between them
			// for each path, replace the tile with a mountain tile
			MovingObjectPath mRange = MovingObjectPath.GetAnyPathTo(startMountain, endMountain);
			Vector3Int currPos = startMountain;
			while(currPos != endMountain) {
				currPos = mRange.Next(currPos);
				TileSetter(currPos, TileOption(TileEnum.mountain));
				GameManager.inst.overworld.SetTerrainAt(currPos, new Mountain(currPos));
			}
		}
	}

	private void CreateForests(int lowerBound = 4, int upperBound = 9) {
		// choose random grass points to make into lakes
		List<Vector3Int> posList = PositionsOfType(TileEnum.forest);
		foreach (var frt in posList) { GameManager.inst.overworld.SetTerrainAt(frt, new Forest(frt)); }

		// flood fill each of these areas with randomized tile counts
		foreach(Vector3Int origin in posList) {
			int forestRange = Random.Range(lowerBound, upperBound);
			int forestSize = Random.Range(lowerBound, upperBound);
			
			var nonMnt = GameManager.inst.overworld.LocationsExceptOf<Mountain>();
			FlowField fField = FlowField.FlowFieldFrom(origin, nonMnt, range: forestRange*Constants.standardTickCost, numElements: forestSize);
			
			foreach(Vector3Int frt in fField.field.Keys) {
				TileSetter(frt, TileOption(TileEnum.forest));
				GameManager.inst.overworld.SetTerrainAt(frt, new Forest(frt));
			}
		}
	}
	
	private void CreateLakes(int lowerBound = 4, int upperBound = 9) {
		// choose random grass points to make into lakes
		List<Vector3Int> posList = PositionsOfType(TileEnum.water);
		foreach (var wtr in posList) { GameManager.inst.overworld.SetTerrainAt(wtr, new Water(wtr)); }

		// flood fill each of these areas with randomized tile counts
		foreach(Vector3Int lakeOrigin in posList) {
			int lakeRange = Random.Range(lowerBound, upperBound);
			int lakeSize = Random.Range(lowerBound, upperBound);
			
			var nonMnt = GameManager.inst.overworld.LocationsExceptOf<Mountain>();
			FlowField fField = FlowField.FlowFieldFrom(lakeOrigin, nonMnt, range: lakeRange*Constants.standardTickCost, numElements: lakeSize);
			foreach(Vector3Int lakePos in fField.field.Keys) {
				TileSetter(lakePos, TileOption(TileEnum.water));
				GameManager.inst.overworld.SetTerrainAt(lakePos, new Water(lakePos));
			}
		}

		// now that the tiles are placed, check for deep water
		foreach(Vector3Int tilePos in GameManager.inst.overworld.LocationsOf<Water>()) {
			bool surrounded = true;
			foreach (var neighbor in GameManager.inst.overworld.GetEightNeighbors(tilePos)) {
				surrounded &= (GameManager.inst.overworld.TypeAt(neighbor).MatchesType( typeof(Plain) ) &&
							   GameManager.inst.overworld.TypeAt(neighbor).MatchesType( typeof(Forest) ));
			}
			if (surrounded) TileSetter(tilePos, TileOption(TileEnum.deepWater));
		}
	}

	// wow I was very lazy when I implemented this
	// just keep it man, there's too much to do
	private void PlaceVillages(int num) {
		if (num < 2) {
			Debug.Log("Need to place at least a starting and ending village");
			return;
		}

		// first village
		// the rest are randomized
		Vector3Int firstVillagePos = new Vector3Int(1, Random.Range(1, mapDimension.y-1), 0);
		TileSetter(firstVillagePos, TileOption(TileEnum.village));
		GameManager.inst.overworld.SetTerrainAt(firstVillagePos, new Village(firstVillagePos));

		Vector3Int lastVillagePos  = new Vector3Int(mapDimension.x-1, Random.Range(1, mapDimension.y-1), 0);
		TileSetter(lastVillagePos, TileOption(TileEnum.village));
		GameManager.inst.overworld.SetTerrainAt(lastVillagePos, new Village(lastVillagePos));

		foreach (Vector3Int villagePos in PositionsOfType(TileEnum.plain).RandomSelections<Vector3Int>(num-2)) {
			TileSetter(villagePos, TileOption(TileEnum.village));
			GameManager.inst.overworld.SetTerrainAt(villagePos, new Village(villagePos));
		}
	}

	private List<Vector3Int> PositionsOfType(TileEnum type) {
		List<Vector3Int> positionList = new List<Vector3Int>();
		
		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == type) {
					positionList.Add(new Vector3Int(x, y, 0));
				}
			}
		}
		return positionList;
	}
	
	private Vector3Int ClosestOfType(Vector3Int startPos, TileEnum type) {
		Vector3Int retVal = startPos;
		float currDist = (float)(map.GetLength(0) + 1);
		
		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == type) {
					Vector3Int currPos = new Vector3Int(x, y, 0);
					if (currPos == startPos) continue;
					
					float dist = Vector3Int.Distance(startPos, currPos);
					if (dist < currDist) {
						currDist = dist;
						retVal = currPos;
					}
				}
			}
		}
		return retVal;
	}
}