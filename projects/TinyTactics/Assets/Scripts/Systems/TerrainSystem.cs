using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Extensions;

public class TerrainSystem : MonoBehaviour
{
    public UnityEvent<TerrainTile> TerrainMouseOverEvent;
    public UnityEvent<GridPosition> TerrainChangeEvent;

    [SerializeField] private Tilemap terrainTilemap;    // every tile in here should be a TerrainTile

    // this class exists to be an active GameObject in the scene,
    // to have children register themselves to various flags
    // this allows for nice, separable components
    public static TerrainSystem inst = null; // enforces singleton behavior
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
    }

    public TerrainTile TerrainAt(GridPosition gp) {
        return (terrainTilemap.GetTile(gp) as TerrainTile);
    }

    public void CheckTerrainMouseOver(GridPosition gp) {
        TerrainTile terrain = TerrainAt(gp);
        if (terrain != null) TerrainMouseOverEvent?.Invoke(terrain);
    }

    public void ApplyExitTerrainEffect(Unit unit, GridPosition gridPosition) {
        TerrainTile exitingTile = TerrainAt(gridPosition);
        if (exitingTile?.HasTerrainEffect ?? false) exitingTile.terrainEffect.OnExitTerrain(unit);
    }

    public void ApplyEnterTerrainEffect(Unit unit, GridPosition gridPosition) {
        TerrainTile enteringTile = TerrainAt(gridPosition);
        if (enteringTile?.HasTerrainEffect ?? false) enteringTile.terrainEffect.OnEnterTerrain(unit);
    }

    // what happens when you apply a terrain effect
    // basically, take the old terrain, push it to the VisualTilemap
    // then add the new terrain on top
    public void CreateTerrainAt(GridPosition gp, TerrainTile terrain) {
        TerrainTile oldTerrain = TerrainAt(gp);
        // visualTilemap.SetTile(gp, oldTerrain);
        terrainTilemap.SetTile(gp, terrain);

        TerrainChangeEvent?.Invoke(gp);
    }
}
