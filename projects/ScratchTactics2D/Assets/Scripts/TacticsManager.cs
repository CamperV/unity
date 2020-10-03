using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TacticsManager : MonoBehaviour
{
	public TacticsGrid tacticsGridPrefab;
	//
	[HideInInspector] public TacticsGrid tacticsGrid;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
	
	public void NewBattle(List<MovingObject> participants, List<WorldTile> tiles) {
		Debug.Assert(tiles.Count == participants.Count);
		var playerParticipant = participants[0];
		
		tacticsGrid = Instantiate(tacticsGridPrefab, playerParticipant.transform.position, Quaternion.identity);
		
		// determine orientations
		Dictionary<Vector3Int, List<Vector3Int>> orientationDict = new Dictionary<Vector3Int, List<Vector3Int>>() {
			[Vector3Int.up] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(tacticsGrid.mapDimensionX, 0, 0)
			}, 
			[Vector3Int.right] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(0, -tacticsGrid.mapDimensionY, 0)
			}, 
			[Vector3Int.down] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(-tacticsGrid.mapDimensionX, 0, 0)
			}, 
			[Vector3Int.left] = new List<Vector3Int>() {
				Vector3Int.zero,
				new Vector3Int(0, tacticsGrid.mapDimensionY, 0)
			}
		};
		var orientation = orientationDict[(participants[1].gridPosition - playerParticipant.gridPosition)];
		
		// setup up each side
		int participantIndex;
		for (participantIndex = 0; participantIndex < participants.Count; participantIndex++) {
			MovingObject participant = participants[participantIndex];
			WorldTile participantTile = tiles[participantIndex];
			Vector3Int orientationOffset = orientation[participantIndex];
			
			tacticsGrid.CreateTacticsMap(orientationOffset, participantTile.GetType());
		}
		
	}
	
	void FinishBattle() {
		tacticsGrid.gameObject.SetActive(false);
	}
}
