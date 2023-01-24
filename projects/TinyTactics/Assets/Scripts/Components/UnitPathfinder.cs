using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitPathfinder : MonoBehaviour
{
	private UnitMap unitMap;
	private BattleMap battleMap;
	private TerrainSystem terrainSystem;

    public TerrainTile[] terrainTiles;
    public int[] _terrainCostOverrides;
	
    public Dictionary<TerrainTile, int> terrainCostOverrides;

	public bool moveThroughEnemiesOverride; // defaults to false
	public bool moveThroughTerrainOverride; // defaults to false
	public bool loweredTerrainCostOverride; // defaults to false

	// get your own IPathable
	void Awake(){
		Battle _topBattleRef = GetComponentInParent<Battle>();
		unitMap   = _topBattleRef.GetComponentInChildren<UnitMap>();
		battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();
		terrainSystem = _topBattleRef.GetComponentInChildren<TerrainSystem>();

	    Debug.Assert(terrainTiles.Length == _terrainCostOverrides.Length);
        terrainCostOverrides = new Dictionary<TerrainTile, int>();

        for (int o = 0; o < terrainTiles.Length; o++) {
            TerrainTile tt = terrainTiles[o];
            terrainCostOverrides[tt] = _terrainCostOverrides[o];
        }
	}

	public T GenerateFlowField<T>(GridPosition startPosition, int range = Int32.MaxValue) where T : FlowField<GridPosition>, new() {
		Dictionary<GridPosition, int> distance = new Dictionary<GridPosition, int>();
		PriorityQueue<GridPosition> fieldQueue = new PriorityQueue<GridPosition>();

		// initial setup
		GridPosition currentPos = startPosition;
		fieldQueue.Enqueue(0, currentPos);
		distance[startPosition] = 0;
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (GridPosition adjacent in battleMap.GetNeighbors(currentPos)) {
				
				////////////////////////////////////////////////////////////
				// Terrain movement constraints (variable/override costs) //
				////////////////////////////////////////////////////////////
				int costAt = -1; // default to impassable

				// if you're flying:
				if (moveThroughTerrainOverride) {
					costAt = 1;
				} else {
					TerrainTile terrainAt = terrainSystem.TerrainAt(adjacent);
					costAt = (terrainCostOverrides.ContainsKey(terrainAt)) ? terrainCostOverrides[terrainAt] : terrainAt.cost;

					if (costAt > 1 && loweredTerrainCostOverride) costAt = 1;
				}
				
				if (costAt == -1) // -1 indicates this area is impassable
					continue;
				
				//////////////////////////////////////////////////////
				// Move distance, ie range v total cost constraints //
				//////////////////////////////////////////////////////
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				var updatedCost = distSoFar + costAt;
				if (updatedCost > range) // the unit can't move this far
					continue;

				////////////////////////////////////////////////////////////////////////////////
				// Check UnitMap to make sure you can/can't pass through units standing there //
				// important distinction: some Units can be moved through,					  //
				// but there can never be two Units are the same location					  //
				// ie, there can never been another Unit at the targetPosition 				  //
				// This distinction is made in the moveRange.BFS pathfinder. Here, we simply  //
				// add all GP to the field if they don't contain enemyUnits					  //
				////////////////////////////////////////////////////////////////////////////////
				if (!moveThroughEnemiesOverride) {
					var otherUnit = unitMap.UnitAt(adjacent);
					if (otherUnit != null) {
						if (GetComponent<Unit>().GetType() != otherUnit.GetType()) // PlayerUnit != EnemyUnit
							continue;
					}
				}

				
				// if you made it through the constraints gauntlet, save the best distance to this GP
				// and enqueue the next search position
				if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
					distance[adjacent] = updatedCost;
					fieldQueue.Enqueue(distance[adjacent], adjacent);
				}
			}
		}
		
		// upon success, create and return a FlowField using the distance dict
		T flowField = new T();
		flowField.origin = startPosition;
		flowField.field = distance;
		return flowField;
	}

	public T GenerateFlowField<T>(GridPosition startPosition, ICollection<GridPosition> dimensions) where T : FlowField<GridPosition>, new() {
		Dictionary<GridPosition, int> distance = new Dictionary<GridPosition, int>();
		PriorityQueue<GridPosition> fieldQueue = new PriorityQueue<GridPosition>();

		// initial setup
		GridPosition currentPos = startPosition;
		fieldQueue.Enqueue(0, currentPos);
		distance[startPosition] = 0;
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (GridPosition adjacent in battleMap.GetNeighbors(currentPos)) {
				////////////////////////////////////////
				// Check dimensions, ie cookie-cutter //
				////////////////////////////////////////
				if (!dimensions.Contains(adjacent))
					continue;
				
				////////////////////////////////////////////////////////////
				// Terrain movement constraints (variable/override costs) //
				////////////////////////////////////////////////////////////
				TerrainTile terrainAt = terrainSystem.TerrainAt(adjacent);
				int costAt = (terrainCostOverrides.ContainsKey(terrainAt)) ? terrainCostOverrides[terrainAt] : terrainAt.cost;
				if (costAt == -1) // -1 indicates this area is impassable
					continue;
				
				///////////////////
				// Move distance //
				///////////////////
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				var updatedCost = distSoFar + costAt;

				////////////////////////////////////////////////////////////////////////////////
				// Check UnitMap to make sure you can/can't pass through units standing there //
				// important distinction: some Units can be moved through,					  //
				// but there can never be two Units are the same location					  //
				// ie, there can never been another Unit at the targetPosition 				  //
				// This distinction is made in the moveRange.BFS pathfinder. Here, we simply  //
				// add all GP to the field if they don't contain enemyUnits					  //
				////////////////////////////////////////////////////////////////////////////////
				if (!moveThroughEnemiesOverride) {
					var otherUnit = unitMap.UnitAt(adjacent);
					if (otherUnit != null) {
						if (GetComponent<Unit>().GetType() != otherUnit.GetType()) // PlayerUnit != EnemyUnit
							continue;
					}
				}

				
				// if you made it through the constraints gauntlet, save the best distance to this GP
				// and enqueue the next search position
				if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
					distance[adjacent] = updatedCost;
					fieldQueue.Enqueue(distance[adjacent], adjacent);
				}
			}
		}
		
		// upon success, create and return a FlowField using the distance dict
		T flowField = new T();
		flowField.origin = startPosition;
		flowField.field = distance;
		return flowField;
	}
}
