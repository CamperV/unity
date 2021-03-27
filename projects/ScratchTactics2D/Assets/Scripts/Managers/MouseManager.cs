﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class MouseManager : MonoBehaviour
{
	private GameGrid currentActiveGrid;
	
	[HideInInspector] public Vector3 mouseWorldPos;

    void Update() {
		// get the collision point of the ray with the z = 0 plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mouseWorldPos = ray.GetPoint(-ray.origin.z / ray.direction.z);
		currentActiveGrid = GameManager.inst.GetActiveGrid();
		
		// debug
		/*
		if (Input.GetMouseButtonDown(0)) {
			if (GameManager.inst.gameState == Enum.GameState.battle) {
				var gridPos = GetValidIsometricGridPos( (currentActiveGrid as TacticsGrid) );
				Debug.Log($"gridPos: {gridPos}");
			}
		}
		*/
    }

	public Vector3Int? GetValidIsometricGridPos(TacticsGrid grid) {
		Ray zRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 zPoint = zRay.GetPoint(-zRay.origin.z / zRay.direction.z);
		Vector3 invOrigin = new Vector3(zRay.origin.x, zRay.origin.y, -zRay.origin.z);

		foreach (var s in invOrigin.SteppedInterpInc(zPoint, 25)) {
			Vector3Int currentGridPos = currentActiveGrid.Real2GridPos(s);
			if (grid.IsInBounds(currentGridPos)) {
				return currentGridPos;
			}
		}
		return null;
	}
}