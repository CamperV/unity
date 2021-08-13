using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Extensions;
using Random = UnityEngine.Random;

public class Battle : MonoBehaviour
{
	private Dictionary<Army, Controller> activeControllers;
	private Dictionary<Controller, Army> activeParticipants;
	//
	public Controller defaultControllerPrefab;
	[HideInInspector] public Controller defaultController;
	//
	[HideInInspector] public TacticsGrid grid;
	private BattleMap battleMap;
	
	[HideInInspector] public PlayerArmy player;
	[HideInInspector] public EnemyArmy other;
	[HideInInspector] public List<Army> allOther;

	[HideInInspector] public Vector3Int playerGridOffset;

	// used for pausing the battle
	[HideInInspector] public int savedTurn;
	[HideInInspector] public bool isPaused = false;
	
	void Awake() {
		grid = GetComponentsInChildren<TacticsGrid>()[0];
		//
		activeControllers = new Dictionary<Army, Controller>();
		defaultController = Instantiate(defaultControllerPrefab);
		defaultController.transform.SetParent(transform);

		activeParticipants = new Dictionary<Controller, Army>();
	}
	
	public void Init(PlayerArmy playerEntity, EnemyArmy otherEntity) {
		player = playerEntity;
		other = otherEntity;

		other.state = Enum.EnemyArmyState.inBattle;
		allOther = new List<Army>{ other };

		// register controllers for units to be registered to
		// we fully re-instantiate controllers each time. Including the player
		// we don't necessarily want OverworldEntities to have unit controllers outside of a battle
		// we do, however, need to keep track of the current units available to each entity better
		foreach (Army participant in new List<Army>{ player, other }) {
			var prefab = participant.unitControllerPrefab;
			if (prefab != null) {
				var controller = Instantiate(prefab);
				controller.transform.SetParent(transform);

				// TODO: this might not need to exist
				activeControllers[participant] = controller;
				activeParticipants[controller] = participant;
			}
		}
	}
	public void PostInit() {
		battleMap.gameObject.SetActive(false);
	}

	public void StartBattleOnPhase(Enum.Phase startingPhase) {
		GameManager.inst.phaseManager.StartPhase(startingPhase);
		
		UIManager.inst.EnableBattlePhaseDisplay(true);
		GameManager.inst.phaseManager.currentTurn = 1;
		GetControllerFromPhase(startingPhase).TriggerPhase();
	}

	public void LoadBattleMap(Terrain playerTerrain, Terrain otherTerrain) {
		string terrainDesignator = $"{playerTerrain.tag}:{otherTerrain.tag}";
		Debug.Log($"Looking up desig {terrainDesignator}");
		List<BattleMap> battleMaps = BattleMapGenerator.GetMapsFromDesignator(terrainDesignator);

		// our current method: random
		// note that we need to Instantiate to get certain fields from the Components
		// otherwise the GO is marked as inactive and we can't query it
		battleMap = Instantiate( battleMaps.PopRandom<BattleMap>() );

		Vector3Int orientation = playerTerrain.position - otherTerrain.position;
		battleMap.RotateBattleMap(orientation);
		BattleMapGenerator.ApplyMap(battleMap, grid.SetAppropriateTile);
		
		// clean up
		grid.baseTilemap.CompressBounds();
		grid.baseTilemap.RefreshAllTiles();
		
		// determine correct centering factor
		// move to center after the tilemap has been filled
		Vector3 gridCenter = grid.GetGridCenterReal();
		Vector3 offsetPos = transform.position - (gridCenter - transform.position);
		
		grid.transform.position = offsetPos;
	}

	public void SpawnAllUnits() {
		Zone playerSpawnZone = battleMap.GetSpawnZoneFromOrientation(other.gridPosition - player.gridPosition);
		Zone otherSpawnZone = battleMap.GetSpawnZoneFromOrientation(player.gridPosition - other.gridPosition);

		SpawnUnits(player, playerSpawnZone);
		SpawnUnits(other, otherSpawnZone);
	}

	private void SpawnUnits(Army army, Zone spawnZone) {		
		// do spawn-y things and add them to the activeUnit registry
		// in the future, assign them to a Director (either player control or AI)
		var spawnPositions = spawnZone.Positions.ToList().RandomSelections<Vector3Int>(army.numUnits);

		// the army will maintain a barracks of units
		// the army has reference to each prefab needed, so we instantiate a prefab here
		// then apply the actual relevant stats
		foreach (UnitState unitStats in army.GetUnits()) {
			var uPrefab = army.LoadUnitByTag("Units/" + unitStats.unitTag);
			Unit unit = TacticsEntityBase.Spawn<Unit>(uPrefab, spawnPositions.PopAt(0), grid) as Unit;
			//
			unit.ApplyStats(unitStats);
			GetController(army).Register(unit);
		}
	}

	public void AddParticipant(Army joiningEntity, Terrain joiningTerrain) {
	}

	public void Resolve(List<Army> defeatedEntities) {
		foreach (var defeatedEntity in defeatedEntities) {
			defeatedEntity.Die();
		}
		Destroy();
	}
	
	public void Destroy() {
		Destroy(gameObject);
		//
		MenuManager.inst.CleanUpBattleMenus();
		//
		UIManager.inst.EnableBattlePhaseDisplay(false);
		GameManager.inst.EnterOverworldState();
	}

	private Controller GetController(Army oe) {
		if (activeControllers.ContainsKey(oe)) {
			return activeControllers[oe];
		} else {
			return defaultController;
		}
	}

	public List<Controller> GetActiveControllers() {
		return activeControllers.Values.ToList();
	}

	public List<MovingGridObject> GetRegisteredInBattle() {
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

	public Army GetArmyFromController(Controller con) {
		if (activeParticipants.ContainsKey(con)) {
			return activeParticipants[con];
		} else {
			return null;
		}
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

	public List<Army> GetDefeated() {
		UnitController playerController = activeControllers[player] as UnitController;
		if (!playerController.activeRegistry.Any()) {
			return new List<Army>{ (Army)player };
		}

		UnitController enemyController = activeControllers[other] as UnitController;
		if (!enemyController.activeRegistry.Any()) {
			return allOther;
		}

		// this should be unreachable code
		Debug.Assert(false);
		return new List<Army>(); // battle continues		
	}

	public void InvisibleFor(float fixedTime) {
		ColorAll(Color.white.WithAlpha(0.0f));
		StartCoroutine( Utils.DelayedExecute(fixedTime, () => ColorAll(Color.white)) );
	}

	private void ColorAll(Color color) {
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sr in renderers) {
			sr.color = color;
		}
		Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
		foreach (Tilemap tm in tilemaps) {
			foreach (Vector3Int pos in GameGrid.GetPositions(tm)) {
				grid.TintTile(tm, pos, color);
			}
		}
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
			GameManager.inst.overworld.DisableTint();
		}));
	}

	public void Resume() {
		isPaused = false;
		gameObject.SetActive(true);

		// tint overworld to give focus to battle
		GameManager.inst.overworld.EnableTint();
		GameManager.inst.overworld.ClearOverlayTiles();
		GameManager.inst.gameState = Enum.GameState.battle;
	}
}
