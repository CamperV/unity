using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TacticsManager : MonoBehaviour
{
	public TacticsGrid tacticsGridPrefab;
	//
	[HideInInspector] public TacticsGrid tacticsGrid;

    void Update() {
		
		// while "in-battle" wait for key commands to exit state
		if (GameManager.inst.gameState == Enum.GameState.battle) {
			if (Input.GetKeyDown("space")) {
				Debug.Log("Exiting Battle...");
				
				FinishBattle();
			}

		}
        
    }
	
	public void NewBattle(List<MovingObject> participants, List<WorldTile> tiles) {
		Debug.Assert(tiles.Count == participants.Count);
		
		var player = participants[0];
		var playerTile = tiles[0];
		
		var other = participants[1];
		var otherTile = tiles[1];
		
		// move to center after the tilemap has been filled
		var cameraPos = new Vector3(Camera.main.transform.position.x,
									Camera.main.transform.position.y,
									0);
		tacticsGrid = Instantiate(tacticsGridPrefab, cameraPos, Quaternion.identity);
		
		// determine orientations
		Dictionary<Vector3Int, List<Vector3Int>> orientationDict = new Dictionary<Vector3Int, List<Vector3Int>>() {
			[Vector3Int.up] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(playerTile.battleGridSize.x, 0, 0)
			}, 
			[Vector3Int.right] = new List<Vector3Int>() {
				new Vector3Int(0, otherTile.battleGridSize.y, 0),
				Vector3Int.zero
			}, 
			[Vector3Int.down] = new List<Vector3Int>() {
				new Vector3Int(otherTile.battleGridSize.x, 0, 0),
				Vector3Int.zero
			}, 
			[Vector3Int.left] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(0, playerTile.battleGridSize.y, 0)
			}
		};
		var orientation = orientationDict[(other.gridPosition - player.gridPosition)];
		
		// setup up each side
		int participantIndex;
		for (participantIndex = 0; participantIndex < participants.Count; participantIndex++) {
			MovingObject participant = participants[participantIndex];
			WorldTile participantTile = tiles[participantIndex];
			Vector3Int orientationOffset = orientation[participantIndex];
			
			// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
			tacticsGrid.CreateTileMap(orientationOffset, participantTile);
		}
		
		// after all participants have generated their TileMaps, apply the contents of the tacticsTileGrid to the baseTilemap
		// then compress the bounds afterwards
		tacticsGrid.ApplyTileMap();
		
		// determine correct centering factor
		Vector3 gridCenter = tacticsGrid.GetGridCenterReal();
		Vector3 offsetPos = cameraPos - (gridCenter - cameraPos);
		
		tacticsGrid.transform.position = offsetPos;
	}
	
	private void FinishBattle() {
		Destroy(tacticsGrid.gameObject);
		
		GameManager.inst.EnterOverworldState();
	}
}
