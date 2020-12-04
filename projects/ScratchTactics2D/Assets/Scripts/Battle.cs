using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
	private Dictionary<OverworldEntity, Controller> activeControllers;
	//
	public Controller defaultControllerPrefab;
	[HideInInspector] public Controller defaultController;
	//
	[HideInInspector] public List<OverworldEntity> worldParticipants;
	[HideInInspector] public List<Unit> activeUnits;
	[HideInInspector] public TacticsGrid grid;
	//
	[HideInInspector] public int currentTurn;
	
	public OverworldEntity player { get { return worldParticipants[0]; } }
	public OverworldEntity other  { get { return worldParticipants[1]; } }
	
	void Awake() {
		worldParticipants = new List<OverworldEntity>();
		grid = GetComponentsInChildren<TacticsGrid>()[0];
		//
		activeControllers = new Dictionary<OverworldEntity, Controller>();
		defaultController = Instantiate(defaultControllerPrefab);
		defaultController.transform.parent = this.transform;
	}
	
	public void Init(List<OverworldEntity> participants, List<WorldTile> tiles) {
		worldParticipants = participants;
		//
		PopulateGridAndReposition(tiles);
		//
		// register controllers for units to be registered to
		foreach (OverworldEntity participant in worldParticipants) {
			var prefab = participant.unitControllerPrefab;
			if (prefab != null) {
				activeControllers[participant] = Instantiate(prefab);
				activeControllers[participant].transform.parent = this.transform;
			}
		}
		//
		SpawnAllUnits();
	}

	public void StartBattleOnPhase(Enum.Phase startingPhase) {
		GameManager.inst.phaseManager.StartPhase(startingPhase);
	}
	
	// we can only start a Battle with two participants
	// however, others can join(?)
	private void PopulateGridAndReposition(List<WorldTile> tiles) {		
		// player is always index 0
		WorldTile playerTile = tiles[0];
		WorldTile otherTile = tiles[1];
		
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
		for (participantIndex = 0; participantIndex < worldParticipants.Count; participantIndex++) {
			OverworldEntity participant = worldParticipants[participantIndex];
			WorldTile participantTile = tiles[participantIndex];
			Vector3Int orientationOffset = orientation[participantIndex];
			
			// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
			grid.CreateTileMap(orientationOffset, participantTile);
		}
		
		// after all worldParticipants have generated their TileMaps, apply the contents of the tacticsTileGrid to the baseTilemap
		// then compress the bounds afterwards
		grid.ApplyTileMap();
		
		// determine correct centering factor
		// move to center after the tilemap has been filled
		Vector3 gridCenter = grid.GetGridCenterReal();
		Vector3 offsetPos = transform.position - (gridCenter - transform.position);
		
		grid.transform.position = offsetPos;
	}
	
	private void SpawnAllUnits() {
		// number of spawnZones is equal to the number of worldParticpants (2)
		List<List<Vector3Int>> spawnZones = GetSpawnZones();
		var playerSpawnZone = spawnZones[0];
		var otherSpawnZone  = spawnZones[1];
		
		// LoadUnitsByTag will look up if an appropriate prefab has already been loaded from the Resources folder
		// if it has, it will instantiate it. If not, it will load first	
		var pUnits = player.LoadUnitsByTag(player.defaultUnitTags);
		var oUnits = other.LoadUnitsByTag(other.defaultUnitTags);
		
		// do spawn-y things and add them to the activeUnit registry
		// in the future, assign them to a Director (either player control or AI)
		var playerSpawnPositions = Utils.RandomSelections<Vector3Int>(playerSpawnZone, pUnits.Count);
		for (int i=0; i<pUnits.Count; i++) {
			Unit unit = (Unit)TacticsEntityBase.Spawn(pUnits[i], playerSpawnPositions[i], grid);
			GetController(player).Register(unit);
			activeUnits.Add(unit);
		}
		
		var otherSpawnPositions = Utils.RandomSelections<Vector3Int>(otherSpawnZone, oUnits.Count);
		for (int i=0; i<oUnits.Count; i++) {
			Unit unit = (Unit)TacticsEntityBase.Spawn(oUnits[i], otherSpawnPositions[i], grid);
			GetController(player).Register(unit);
			activeUnits.Add(unit);
		}
	}
	
	private List<List<Vector3Int>> GetSpawnZones() {
		Vector3Int playerA = Vector3Int.zero;
		Vector3Int playerB = Vector3Int.zero;
		Vector3Int otherA  = Vector3Int.zero;
		Vector3Int otherB  = Vector3Int.zero;
		
		// these are the maximum size in each direction
		Vector3Int gridDim = grid.GetDimensions() - Vector3Int.one;
		
		// boy this is a dumb, stubborn way to do this
		switch (other.gridPosition - player.gridPosition) {
			case Vector3Int v when v.Equals(Vector3Int.up):
				playerA = Vector3Int.zero;
				playerB = new Vector3Int(0, gridDim.y, 0);
				otherA  = new Vector3Int(gridDim.x, 0, 0);
				otherB  = new Vector3Int(gridDim.x, gridDim.y, 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.right):
				playerA = new Vector3Int(0, gridDim.y, 0);
				playerB = new Vector3Int(gridDim.x, gridDim.y, 0);
				otherA  = Vector3Int.zero;
				otherB  = new Vector3Int(gridDim.x, 0, 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.down):
				playerA = new Vector3Int(gridDim.x, 0, 0);
				playerB = new Vector3Int(gridDim.x, gridDim.y, 0);
				otherA  = Vector3Int.zero;
				otherB  = new Vector3Int(0, gridDim.y, 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.left):
				playerA = Vector3Int.zero;
				playerB = new Vector3Int(gridDim.x, 0, 0);
				otherA  = new Vector3Int(0, gridDim.y, 0);
				otherB  = new Vector3Int(gridDim.x, gridDim.y, 0);
				break;
		}

		// create the zone to spawn units into
		// randomly select which starting positions happen, for now
		return new List<List<Vector3Int>>() {
			LineBetweenInclusive(playerA, playerB),
			LineBetweenInclusive(otherA, otherB)
		};
	}
	
	// this is super stupid
	private List<Vector3Int> LineBetweenInclusive(Vector3Int a, Vector3Int b) {
		List<Vector3Int> retVal = new List<Vector3Int>();
		Vector3Int curr = a;
		while (curr != b) {
			retVal.Add(curr);
			Vector3 vcurr = new Vector3(curr.x, curr.y, curr.z);
			Vector3 vb 	  = new Vector3(b.x, b.y, b.z);
			vcurr = Vector3.MoveTowards(vcurr, vb, 1.0f);
			curr = new Vector3Int((int)vcurr.x, (int)vcurr.y, (int)vcurr.z);
		}
		return retVal;
	}

	private Controller GetController(OverworldEntity oe) {
		if (activeControllers.ContainsKey(oe)) {
			return activeControllers[oe];
		} else {
			return defaultController;
		}
	}

	public Controller GetControllerFromPhase(Enum.Phase phase) {
		if (phase == Enum.Phase.player) {
			return GetController(player);
		} else if (phase == Enum.Phase.enemy) {
			return GetController(other);
		} else {
			return defaultController;
		}
	}
}
