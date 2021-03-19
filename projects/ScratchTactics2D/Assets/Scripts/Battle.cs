using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Extensions;

public class Battle : MonoBehaviour
{
	private Dictionary<OverworldEntity, Controller> activeControllers;
	private Dictionary<Controller, OverworldEntity> activeParticipants;
	//
	public Controller defaultControllerPrefab;
	[HideInInspector] public Controller defaultController;
	//
	[HideInInspector] public TacticsGrid grid;
	
	public OverworldPlayer player;
	public OverworldEntity other;
	public List<OverworldEntity> allOther;

	public Vector3Int playerGridOffset;

	// used for pausing the battle
	public int savedTurn;
	public bool isPaused = false;
	
	void Awake() {
		grid = GetComponentsInChildren<TacticsGrid>()[0];
		//
		activeControllers = new Dictionary<OverworldEntity, Controller>();
		defaultController = Instantiate(defaultControllerPrefab);
		defaultController.transform.SetParent(transform);

		activeParticipants = new Dictionary<Controller, OverworldEntity>();
	}
	
	public void Init(OverworldPlayer playerEntity, OverworldEntity otherEntity, WorldTile playerTile, WorldTile otherTile) {
		player = playerEntity;
		other = otherEntity;

		(other as OverworldEnemyBase).state = Enum.EnemyState.inBattle;
		allOther = new List<OverworldEntity>{ other };

		//
		PopulateGridAndReposition(playerTile, otherTile);
		//
		// register controllers for units to be registered to
		// we fully re-instantiate controllers each time. Including the player
		// we don't necessarily want OverworldEntities to have unit controllers outside of a battle
		// we do, however, need to keep track of the current units available to each entity better
		foreach (OverworldEntity participant in new List<OverworldEntity>{ player, other }) {
			var prefab = participant.unitControllerPrefab;
			if (prefab != null) {
				var controller = Instantiate(prefab);
				controller.transform.SetParent(transform);

				// TODO: this might not need to exist
				activeControllers[participant] = controller;
				activeParticipants[controller] = participant;
			}
		}
		//
		SpawnAllUnits();
	}

	public void StartBattleOnPhase(Enum.Phase startingPhase) {
		GameManager.inst.phaseManager.StartPhase(startingPhase);
		
		UIManager.inst.EnableBattlePhaseDisplay(true);
		GameManager.inst.phaseManager.currentTurn = 1;
		GetControllerFromPhase(startingPhase).TriggerPhase();
	}
	
	// we can only start a Battle with two participants
	// however, others can join(?)
	private void PopulateGridAndReposition(WorldTile playerTile, WorldTile otherTile) {				
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
		var offsets = orientationDict[(other.gridPosition - player.gridPosition)];

		// store for later
		playerGridOffset = offsets[0];
		
		// setup up each side	
		// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
		grid.CreateTileMap(offsets[0], playerTile);
		grid.CreateTileMap(offsets[1], otherTile);
		
		// after all battle participants have generated their TileMaps, apply the contents of the tacticsTileGrid to the baseTilemap
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
		Pair<Zone, Zone> spawnZones = GetSpawnZones();
		
		// do spawn-y things and add them to the activeUnit registry
		// in the future, assign them to a Director (either player control or AI)
		var playerSpawnPositions = spawnZones.first.GetPositions().RandomSelections<Vector3Int>(player.barracks.Count);

		// the player will maintain a barracks of units
		// the player has reference to each prefab needed, so we instantiate a prefab here
		// then apply the actual relevant stats
		foreach (UnitStats unitStats in player.barracks.Values) {
			var uPrefab = player.LoadUnitByTag(unitStats.unitTag);
			PlayerUnit unit = (PlayerUnit)TacticsEntityBase.Spawn(uPrefab, playerSpawnPositions.PopAt(0), grid);
			//
			unit.ApplyStats(unitStats);
			GetController(player).Register(unit);
		}

		// LoadUnitsByTag will look up if an appropriate prefab has already been loaded from the Resources folder
		// if it has, it will instantiate it. If not, it will load first
		var otherSpawnPositions = spawnZones.second.GetPositions().RandomSelections<Vector3Int>(other.barracks.Count);

		foreach (UnitStats unitStats in other.barracks.Values) {
			var uPrefab = other.LoadUnitByTag(unitStats.unitTag);
			Unit unit = (Unit)TacticsEntityBase.Spawn(uPrefab, otherSpawnPositions.PopAt(0), grid);
			//
			unit.ApplyStats(unitStats);
			GetController(other).Register(unit);
		}
	}

	// TODO: refactor this into something that is actually modular
	// right now, I'm too smooth-brain
	private Pair<Zone, Zone> GetSpawnZones() {
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
				playerB = new Vector3Int((int)(gridDim.x/4.0f), gridDim.y, 0);
				//
				otherA  = new Vector3Int(gridDim.x, 0, 0);
				otherB  = new Vector3Int(gridDim.x - (int)(gridDim.x/4.0f), gridDim.y, 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.right):
				playerA = new Vector3Int(0, gridDim.y, 0);
				playerB = new Vector3Int(gridDim.x, gridDim.y  - (int)(gridDim.y/4.0f), 0);
				//
				otherA  = Vector3Int.zero;
				otherB  = new Vector3Int(gridDim.x, (int)(gridDim.y/4.0f), 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.down):
				playerA = new Vector3Int(gridDim.x, 0, 0);
				playerB = new Vector3Int(gridDim.x - (int)(gridDim.x/4.0f), gridDim.y, 0);
				//
				otherA  = Vector3Int.zero;
				otherB  = new Vector3Int((int)(gridDim.x/4.0f), gridDim.y, 0);
				break;
			case Vector3Int v when v.Equals(Vector3Int.left):
				playerA = Vector3Int.zero;
				playerB = new Vector3Int(gridDim.x, (int)(gridDim.y/4.0f), 0);
				//
				otherA  = new Vector3Int(0, gridDim.y, 0);
				otherB  = new Vector3Int(gridDim.x, gridDim.y  - (int)(gridDim.y/4.0f), 0);
				break;
		}

		// create the zone to spawn units into
		// randomly select which starting positions happen, for now
		return new Pair<Zone, Zone>(new Zone(playerA, playerB), new Zone(otherA, otherB));
	}

	public void AddParticipant(OverworldEntity joiningEntity, WorldTile joiningTile) {
		(joiningEntity as OverworldEnemyBase).state = Enum.EnemyState.inBattle;
		allOther.Add(joiningEntity);

		// add to grid and reposition
		WorldTile playerTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(player.gridPosition);
		Dictionary<Vector3Int, Vector3Int> orientationDict = new Dictionary<Vector3Int, Vector3Int>() {
			[Vector3Int.up]    = playerGridOffset + new Vector3Int(playerTile.battleGridSize.x, 0, 0),
			[Vector3Int.right] = playerGridOffset - new Vector3Int(0, joiningTile.battleGridSize.y, 0),
			[Vector3Int.down]  = playerGridOffset - new Vector3Int(joiningTile.battleGridSize.x, 0, 0), 
			[Vector3Int.left]  = playerGridOffset + new Vector3Int(0, playerTile.battleGridSize.y, 0)
		};
		var offset = orientationDict[(joiningEntity.gridPosition - player.gridPosition)];
		
		// this Tile's Map gets added to the overall baseTilemap of TacticsGrid
		grid.CreateTileMap(offset, joiningTile);
		grid.ApplyTileMap(noCompress: false);
		//
		Vector3 gridCenter = grid.GetGridCenterReal();
		Vector3 offsetPos = transform.position - (gridCenter - transform.position);
		transform.position = offsetPos;

		// spawn new units
		// register new units to existing controller
		// but... keep tabs, because we need to use this to kill OverworldEntities
		// TODO: kill overworld entities better, I guess
		Controller existingEnemyController = activeControllers[other];
		activeControllers[joiningEntity] = existingEnemyController;
		//

		//
		// get SpawnZone
		//
		Vector3Int A = Vector3Int.zero;
		Vector3Int B = Vector3Int.zero;

		// helpers
		Vector3Int min  = offset;
		Vector3Int xDim = new Vector3Int(joiningTile.battleGridSize.x-1, 0, 0);
		Vector3Int yDim = new Vector3Int(0, joiningTile.battleGridSize.y-1, 0); 
		Vector3Int max  = min + xDim + yDim;
		switch (joiningEntity.gridPosition - player.gridPosition) {
			case Vector3Int v when v.Equals(Vector3Int.up):
				A = max;
				B = max - yDim - xDim.DivBy(4);
				break;
			case Vector3Int v when v.Equals(Vector3Int.right):
				A = min;
				B = min + xDim + yDim.DivBy(4);
				break;
			case Vector3Int v when v.Equals(Vector3Int.down):
				A = min;
				B = min + yDim + xDim.DivBy(4);
				break;
			case Vector3Int v when v.Equals(Vector3Int.left):
				A = max;
				B = max - xDim - yDim.DivBy(4);
				break;
		}
		Zone spawnZone = new Zone(A, B);

		//
		// spawn those units
		// register them 
		//
		var spawnPositions = spawnZone.GetPositions().RandomSelections<Vector3Int>(joiningEntity.barracks.Count);
		foreach (UnitStats unitStats in joiningEntity.barracks.Values) {
			var uPrefab = joiningEntity.LoadUnitByTag(unitStats.unitTag);
			Unit unit = (Unit)TacticsEntityBase.Spawn(uPrefab, spawnPositions.PopAt(0), grid);
			//
			unit.ApplyStats(unitStats);
			existingEnemyController.Register(unit);
		}	
	}

	private Controller GetController(OverworldEntity oe) {
		if (activeControllers.ContainsKey(oe)) {
			return activeControllers[oe];
		} else {
			return defaultController;
		}
	}

	public List<Controller> GetActiveControllers() {
		return activeControllers.Values.ToList();
	}

	public List<MovingObject> GetRegisteredInBattle() {
		return activeControllers.Values.SelectMany(con => con.activeRegistry).ToList();
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

	public OverworldEntity GetOverworldEntityFromController(Controller con) {
		if (activeParticipants.ContainsKey(con)) {
			return activeParticipants[con];
		} else {
			return null;
		}
	}
	
	public OverworldEntity GetOverworldEntityFromPhase(Enum.Phase phase) {
		if (phase == Enum.Phase.player) {
			return player;
		} else if (phase == Enum.Phase.enemy) {
			return other;
		}

		// default,
		Debug.Log($"Phase {phase} is causing a default return for OE");
		return other;
	}

	// this will only work with 2-participant battles, if one side dies, it's over
	// in the future, make some actual decision:
	// a) battles can't happen between two non-players
	// b) there will be explicity variables for players/non-players
	public bool CheckBattleEndState() {
		foreach (UnitController participantController in activeParticipants.Keys) {
			bool alive = participantController.activeRegistry.Any();
			if (!alive) return true;
		}
		return false; // battle continues
	}

	public List<OverworldEntity> GetDefeated() {
		UnitController playerController = activeControllers[player] as UnitController;
		if (!playerController.activeRegistry.Any()) {
			return new List<OverworldEntity>{ (OverworldEntity)player };
		}

		UnitController enemyController = activeControllers[other] as UnitController;
		if (!enemyController.activeRegistry.Any()) {
			return allOther;
		}

		// this should be unreachable code
		Debug.Assert(false);
		return new List<OverworldEntity>(); // battle continues		
	}

	// pause, hand control off to the GameManager.overworld state
	public void Pause() {
		savedTurn = GameManager.inst.phaseManager.currentTurn;
		GameManager.inst.gameState = Enum.GameState.overworld;

		// TODO: when this is above 0, the enemies don't take their turns
		// leave this for the re-write
		StartCoroutine( Utils.DelayedExecute(0.0f, () => {
			isPaused = true;
			gameObject.SetActive(false);
			GameManager.inst.worldGrid.DisableTint();
		}));
	}

	public void Resume() {
		isPaused = false;
		gameObject.SetActive(true);

		// tint overworld to give focus to battle
		GameManager.inst.worldGrid.EnableTint();
		GameManager.inst.worldGrid.ClearOverlayTiles();
		GameManager.inst.gameState = Enum.GameState.battle;
	}
}
