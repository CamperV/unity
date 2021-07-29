using System.Collections;
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
		
		if (Input.GetMouseButtonDown(0)) {
			Vector3Int gridPos = Vector3Int.zero;
			if (GameManager.inst.gameState == Enum.GameState.battle) {
				gridPos = GetMouseToGridPos( (currentActiveGrid as TacticsGrid) ) ?? Vector3Int.zero;
			} else {
				gridPos = currentActiveGrid.Real2GridPos(mouseWorldPos);
			}
			Debug.Log($"gridPos: {gridPos} [{GameManager.inst.overworld.TerrainAt(gridPos)?.GetType()}] [{GameManager.inst.player.fov.OcclusionAt(gridPos)}]");
		}
		/*

		if (GameManager.inst.gameState == Enum.GameState.battle) {
			Vector3? m = ProjectMouseOnGrid(currentActiveGrid);
			if (Input.GetMouseButtonDown(0)) {
				burstZone?.ClearDisplay();

				if (m != null) {
					if (burstZone == null) {
						burstZone = new BurstZone((Vector3)m, 2, (currentActiveGrid as TacticsGrid) );
					}
					burstZone.pivot = (Vector3)m;
					burstZone.RecalculateCoverage();
					burstZone.Display();
				}
			}
		}
		*/

		// foreach (var v in GameManager.inst.overworld.Positions) {
		// 	GameManager.inst.overworld.ResetHighlightTile(v);
		// }
		
		// foreach(var aaPair in FieldOfView.BresenhamLineAA(GameManager.inst.player.gridPosition, currentActiveGrid.Real2GridPos(mouseWorldPos))) {
		// 	Vector3Int ln = aaPair.first;
		// 	float tint = aaPair.second;
		// 	if (GameManager.inst.overworld.IsInBounds(ln)) {
		// 		GameManager.inst.overworld.HighlightTile(ln, tint*Color.white);
				
		// 		// if (ln == currentActiveGrid.Real2GridPos(mouseWorldPos)) {
		// 		// 	// check diagonal squeezers
		// 		// 	GameManager.inst.overworld.HighlightTile(ln - unitDir.X(), Constants.threatColorRed);
		// 		// 	GameManager.inst.overworld.HighlightTile(ln - unitDir.Y(), Constants.threatColorRed);
		// 		// }
		// 	}
		// }
		// Vector3Int unitDir = (currentActiveGrid.Real2GridPos(mouseWorldPos) - GameManager.inst.player.gridPosition).Unit();
		// foreach(Vector3Int ln in FieldOfView.BresenhamLine(GameManager.inst.player.gridPosition, currentActiveGrid.Real2GridPos(mouseWorldPos))) {
		// 	if (GameManager.inst.overworld.IsInBounds(ln)) {
		// 		GameManager.inst.overworld.HighlightTile(ln, Constants.threatColorIndigo);
		// 	}
		// }

		// foreach(Vector3Int ln in FieldOfView.RaycastLine(GameManager.inst.player.gridPosition, currentActiveGrid.Real2GridPos(mouseWorldPos), GameManager.inst.overworld)) {
		// 	if (GameManager.inst.overworld.IsInBounds(ln)) {
		// 		GameManager.inst.overworld.HighlightTile(ln, Constants.threatColorIndigo);
		// 	}
		// }
    }

	public Vector3Int? GetMouseToGridPos(GameGrid grid) {
		Ray zRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 zPoint = zRay.GetPoint(-zRay.origin.z / zRay.direction.z);
		Vector3 invOrigin = new Vector3(zRay.origin.x, zRay.origin.y, -zRay.origin.z);

		foreach (var s in invOrigin.SteppedInterpInc(zPoint, 25)) {
			Vector3Int currentGridPos = grid.Real2GridPos(s);
			if (grid.IsInBounds(currentGridPos)) {
				return currentGridPos;
			}
		}
		return null;
	}

	public Vector3? ProjectMouseOnGrid(GameGrid grid) {
		Ray zRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 zPoint = zRay.GetPoint(-zRay.origin.z / zRay.direction.z);
		Vector3 invOrigin = new Vector3(zRay.origin.x, zRay.origin.y, -zRay.origin.z);

		foreach (var s in invOrigin.SteppedInterpInc(zPoint, 25)) {
			Vector3Int currentGridPos = grid.Real2GridPos(s);
			if (grid.IsInBounds(currentGridPos)) {
				return s;
			}
		}
		return null;
	}
}