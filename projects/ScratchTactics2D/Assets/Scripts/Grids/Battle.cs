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
	// singleton
	public static Battle active = null; // enforces singleton behavior

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

	public IEnumerable<Unit> RegisteredUnits {
		get => activeControllers.Values.SelectMany(con => con.activeRegistry).OfType<Unit>();
	}
	
	// only one Unit can be in focus at any given time
	[HideInInspector] public static Unit focusSingleton { get; private set; }

	// used for pausing the battle
	[HideInInspector] public bool isPaused = false;
	[HideInInspector] public bool hidden = false; // this is for InvisibleFor overriding all other alpha writes
	[HideInInspector] public bool interactable = true;

	// handles construction of Battle and management of tacticsGrid
	// NOTE: we can only ever start a battle with two participants
	// because of the interleaving of Overworld turns and Tactics turns, even if there WOULD be more than one enemy active,
	// it still won't be included in the battle until it can take its turn
	public static void CreateActiveBattle(PlayerArmy player, EnemyArmy other, Terrain playerTerrain, Terrain otherTerrain, Enum.Phase initiatingPhase) {
		var cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);		
		Battle.active = Instantiate(GameManager.inst.battlePrefab, cameraPos, Quaternion.identity);
		Battle.active.Init(player, other);
		Battle.active.LoadBattleMap(playerTerrain, otherTerrain);
		Battle.active.SpawnAllUnits();
		Battle.active.PostInit();
	}
	
	void Awake() {
		// only allow one Battle to exist at any time
		if (active == null) {
			active = this;
		} else if (active != this) {
			Destroy(gameObject);
		}

		grid = GetComponentsInChildren<TacticsGrid>()[0];
		//
		activeControllers = new Dictionary<Army, Controller>();
		defaultController = Instantiate(defaultControllerPrefab, transform);

		activeParticipants = new Dictionary<Controller, Army>();
	}

	void Start() {
		UIManager.inst.EnableBattlePhaseDisplay(true);
		GetComponent<TurnManager>().playerPhase.StartEvent += RefreshRegisteredUnits;
		GetComponent<TurnManager>().enemyPhase.StartEvent  += RefreshRegisteredUnits;
	}

	void Update() {
		// .hidden overrides any transparency modifications that might need to happen in this Update() loop
		if (!this.interactable || this.hidden) return;

		// prematurely destroy battle
		if (Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log("Exiting Battle...");
			this.Destroy();
		}

		// before doing anything, check flags to see if the battle should be terminated
		if (CheckBattleEndState()) {
			Resolve( GetDefeated() );
			return; // jump out of the update loop early
		}

		// focus control:
		// move it all into once-per-frame centralized check, because we can't guarantee 
		// the order in which the other Update()/LateUpdate()s resolve
		Unit newFocus = GetNewFocus();

		// actually set the focus here
		// switch focus if the current focusSingleton is null and no selectionLock is in place
		if (newFocus != focusSingleton) {
			if (focusSingleton == null || !focusSingleton.selectionLock) {
				focusSingleton?.SetFocus(false);
				focusSingleton = newFocus;
				focusSingleton?.SetFocus(true);
			}
		}

		// Ghost Control - UNITS
		var descendingInBattle = RegisteredUnits.OrderByDescending(it => it.gridPosition.y);

		foreach (Unit u in descendingInBattle) {
			bool ghosted = false;
			if (!u.inFocus && !u.inMildFocus) {
				// each unit will check its own processes to see if it should be ghosted
				// having multiple senders, i.e. PathOverlayIso tiles and other Units, is difficult to keep track of

				// if there is any overlay that can be obscured:
				Vector3Int northPos = u.gridPosition.GridPosInDirection(grid, new Vector2Int(1, 1));
				if (grid.GetOverlayAt(u.gridPosition) || grid.GetOverlayAt(northPos)) {
					ghosted = true;
				}
						
				// or, if there is a Unit with an active focus right behind
				var occupantAt = grid.OccupantAt(northPos);
				if (occupantAt?.GetType().IsSubclassOf(typeof(Unit)) ?? false) {
					if ((occupantAt as Unit).inFocus) {
						ghosted = true;
					}
				}
			}

			// specific contextual check for move selections, when the move is behind the PlayerUnit
			if (u.inFocus && u.GetType().IsSubclassOf(typeof(PlayerUnit))) {
				if ((u as PlayerUnit).actionState == Enum.PlayerUnitState.moveSelection) {
					// if there is any overlay that can be obscured (only above the Unit):
					Vector3Int northPos = u.gridPosition.GridPosInDirection(grid, new Vector2Int(1, 1));
					if (grid.GetOverlayAt(northPos)) {
						ghosted = true;
					}
				}
			}

			if (ghosted) {
				u.SetTransparency(0.35f);
				u.clickable = false;
			} else {
				u.SetTransparency(1f);
				u.clickable = true;
			}
		}

		// Ghost Control - TILES
		foreach (Vector3Int surfacePos in grid.Surface) {
			if (grid.DimmableAt(surfacePos)) {
				bool ghosted = false;

				ghosted |= grid.GetOverlayAt(surfacePos);
				ghosted |= !grid.UnderlayNull(surfacePos);
				for (int h = 1; h < grid.ZHeightAt(surfacePos)+1; h++) {
					Vector3Int northPos = surfacePos.GridPosInDirection(grid, new Vector2Int(h, h));
					ghosted |= grid.GetOverlayAt(northPos);
					ghosted |= !grid.UnderlayNull(northPos);
							
					// or, if there is a Unit with an active focus right behind
					ghosted |= grid.OccupantAt(northPos)?.GetType().IsSubclassOf(typeof(Unit)) ?? false;
				}

				if (ghosted) {
					grid.TintTile(grid.baseTilemap, surfacePos, Color.white.WithAlpha(0.35f));
				} else {
					grid.TintTile(grid.baseTilemap, surfacePos, Color.white.WithAlpha(1f));
				}
			}
		}
	}
	
	public void Init(PlayerArmy playerEntity, EnemyArmy otherEntity) {
		player = playerEntity;
		other = otherEntity;

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

	public void RefreshRegisteredUnits() {
		foreach (Unit u in RegisteredUnits) {
			u.UpdateThreatRange();
		}
	}

	public void RecenterGrid() {
		// determine correct centering factor
		// move to center after the tilemap has been filled
		Vector3 gridCenter = grid.GetGridCenterReal();
		Vector3 offsetPos = transform.position - (gridCenter - transform.position);
		
		grid.transform.position = offsetPos;
	}

	public void LoadBattleMap(Terrain playerTerrain, Terrain otherTerrain) {
		string terrainDesignator = $"{playerTerrain.tag}:{otherTerrain.tag}";
		Debug.Log($"Looking up desig {terrainDesignator}");
		List<BattleMap> battleMaps = BattleMapGenerator.GetMapsFromDesignator(terrainDesignator);

		// our current method: random
		// note that we need to Instantiate to get certain fields from the Components
		// otherwise the GO is marked as inactive and we can't query it
		battleMap = Instantiate(battleMaps.PopRandom<BattleMap>());

		Vector3Int orientation = playerTerrain.position - otherTerrain.position;
		battleMap.playerEnemyOrientation = orientation;
		//
		battleMap.RotateBattleMap(orientation);
		BattleMapGenerator.ApplyMap(battleMap, grid.SetAppropriateTile);
		
		// clean up
		grid.baseTilemap.CompressBounds();
		grid.baseTilemap.RefreshAllTiles();
		RecenterGrid();
	}

	public void SpawnAllUnits() {
		Zone playerSpawnZone = battleMap.GetSpawnZoneFromOrientation(player.gridPosition - other.gridPosition);
		Zone otherSpawnZone = battleMap.GetSpawnZoneFromOrientation(other.gridPosition - player.gridPosition);

		var _spawnedPlayer = SpawnUnits(player, playerSpawnZone);
		var _spawnedOther = SpawnUnits(other, otherSpawnZone);
	}

	private List<Unit> SpawnUnits(Army army, Zone spawnZone) {
		List<Unit> retVal = new List<Unit>();

		// do spawn-y things and add them to the activeUnit registry
		// in the future, assign them to a Director (either player control or AI)
		var spawnPositions = spawnZone.Positions.ToList().RandomSelections<Vector3Int>(army.numUnits);

		// the army will maintain a barracks of units
		// the army has reference to each prefab needed, so we instantiate a prefab here
		// then apply the actual relevant stats
		foreach (UnitState unitState in army.GetUnits()) {
			var uPrefab = army.LoadUnitByTag("Units/" + unitState.unitTag);
			Unit unit = TacticsEntityBase.Spawn<Unit>(uPrefab, spawnPositions.PopAt(0), grid) as Unit;
			//
			unit.ApplyState(unitState);
			GetControllerFromTag(army).Register(unit);
			retVal.Add(unit);
		}
		
		return retVal;
	}

	public void AddParticipant(Army joiningEntity, Terrain joiningTerrain) {
		Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(player.gridPosition);
		
		// load correct docker
		string terrainDesignator = $"{playerTerrain.tag}:{joiningTerrain.tag}";
		List<BattleMap> dockerMaps = BattleMapGenerator.GetDockersFromDesignator(terrainDesignator);
		BattleMap docker = Instantiate(dockerMaps.PopRandom<BattleMap>());
		
		// determine orientation to player's terrain
		Vector3Int jOrientation = playerTerrain.position - joiningTerrain.position;
		docker.RotateDocker(jOrientation);

		// reactivate battleMap to query for docking points
		// determine appropriate docking points
		List<Vector3Int> bmDockingPoints = battleMap.GetDockingPointsFromJoiningOrientation(jOrientation).OrderBy(v => v.x).ToList();
		List<Vector3Int> drDockingPoints = docker.GetAllDockingPoints().OrderBy(v => v.x).ToList();
		Debug.Assert(bmDockingPoints.Count == 2);
		Debug.Assert(drDockingPoints.Count == 2);

		Vector3Int dockingAddlOffset = new Vector3Int(-jOrientation.y, jOrientation.x, jOrientation.z);
		Vector3Int dockingOffset = bmDockingPoints[0] - drDockingPoints[0] + dockingAddlOffset;
		Vector3Int dockingOffset2 = bmDockingPoints[1] - drDockingPoints[1] + dockingAddlOffset;
		Debug.Assert(dockingOffset == dockingOffset2);

		// determine the offset appropriate for these
		// add it to the currently active battlemap
		BattleMapGenerator.ApplyMap(docker, grid.SetAppropriateTile, offset: dockingOffset);
		
		// clean up
		grid.baseTilemap.CompressBounds();
		grid.baseTilemap.RefreshAllTiles();
		//RecenterGrid();

		// spawn in the enemies
		// add enemies to the EnemyUnitController which is active
		Zone joiningSpawnZone = docker.GetDockerSpawnZone(dockingOffset);
		List<Unit> spawnedUnits = SpawnUnits(joiningEntity, joiningSpawnZone);

		// do all the application, but freeze the battle state, record the tiles and units,
		// and animate them independently
		float animTime = docker.AnimateDocking(dockingOffset, spawnedUnits, jOrientation);
		StartCoroutine( Utils.DelayedExecute(animTime, () => {
			StartCoroutine( GetComponent<BattleCamera>().Shake(0.75f, dockingAddlOffset) );
		 }) );

		// finally:
		//docker.gameObject.SetActive(false);
	}

	public void Resolve(List<Army> defeatedEntities) {
		foreach (var defeatedEntity in defeatedEntities) {
			defeatedEntity.Die();
		}
		Destroy();
	}
	
	public void Destroy() {
		Destroy(gameObject);
		Battle.active = null;
		//
		MenuManager.inst.CleanUpBattleMenus();
		//
		UIManager.inst.EnableBattlePhaseDisplay(false);
		GameManager.inst.overworld.GetComponent<TurnManager>().Resume();
		GameManager.inst.EnterOverworldState();
	}

	private Unit GetNewFocus() {
		// Focus control: reset if applicable and highlight/focus
		var unitsInBattle = RegisteredUnits.OrderBy(it => it.gridPosition.y);
		foreach (Unit u in unitsInBattle) {
			if (u.clickable && u.mouseOver) {
				return u;
			}
		}
		
		// secondary try: select based on tileGridPos AFTER determining BB collisions
		foreach (Unit u in unitsInBattle) {
			if (!u.spriteAnimator.isMoving && grid.GetMouseToGridPos() == u.gridPosition) {
				return u;
			}
		}

		return null;
	}

	private Unit _Deprecated_GetNewFocus() {
		// Focus control: reset if applicable and highlight/focus
		var unitsInBattle = RegisteredUnits.OrderBy(it => it.gridPosition.y);
		foreach (Unit u in unitsInBattle) {
			if (!u.clickable /* && u.ColliderContains(mm.mouseWorldPos) */) {
				return u;
			}
		}
		
		// secondary try: select based on tileGridPos AFTER determining BB collisions
		foreach (Unit u in unitsInBattle) {
			if (!u.spriteAnimator.isMoving && grid.GetMouseToGridPos() == u.gridPosition) {
				return u;
			}
		}

		return null;
	}

	private Controller GetController(Army oe) {
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

	public Controller GetControllerFromTag(Army oe) {
		switch (oe.armyTag) {
			case "PlayerArmy":
				return GetController(player);
			case "EnemyArmy":
				return GetController(other);
			default:
				return defaultController;
		}
	}

	public Controller GetControllerFromTag(string armyTag) {
		switch (armyTag) {
			case "PlayerArmy":
				return GetController(player);
			case "EnemyArmy":
				return GetController(other);
			default:
				return defaultController;
		}
	}

	public List<Controller> GetActiveControllers() {
		return activeControllers.Values.ToList();
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
		StartCoroutine( _InvisibleFor(fixedTime) );
	}

	private IEnumerator _InvisibleFor(float fixedTime) {
		hidden = true;
		interactable = false;
		Color _invis = Color.white.WithAlpha(0f);

		// hide them all, then wait, then un-hide
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

		// this is for re-coloring the tileMaps
		Dictionary<SpriteRenderer, Color> srColors = new Dictionary<SpriteRenderer, Color>();
		Dictionary<Tilemap, Dictionary<Vector3Int, Color>> tmColors = new Dictionary<Tilemap, Dictionary<Vector3Int, Color>>();

		foreach (SpriteRenderer sr in renderers) {
			srColors[sr] = sr.color;
			sr.color = _invis;
		}
		foreach (Tilemap tm in tilemaps) {
			Dictionary<Vector3Int, Color> tileColors = new Dictionary<Vector3Int, Color>();

			foreach (Vector3Int pos in GameGrid.GetPositions(tm)) {
				tileColors[pos] = grid.GetTint(tm, pos);
				grid.TintTile(tm, pos, _invis);
			}
			tmColors[tm] = tileColors;
		}

		yield return new WaitForSeconds(fixedTime);

		// now re-color with saved colors
		foreach (SpriteRenderer sr in srColors.Keys) {
			sr.color = srColors[sr];
		}
		foreach (Tilemap tm in tmColors.Keys) {
			var tileColors = tmColors[tm];

			foreach (Vector3Int pos in tileColors.Keys) {
				grid.TintTile(tm, pos, tileColors[pos]);
			}
		}
		
		interactable = true;
		hidden = false;
	}

	// pause, hand control off to the GameManager.overworld state
	public void Pause() {
		Debug.Log($"paused battle");
		isPaused = true;
		hidden = true;
		interactable = false;
		GameManager.inst.overworld.DisableTint();
	}

	public void Resume() {
		Debug.Log($"resumed battle");
		isPaused = false;
		hidden = false;
		interactable = true;

		// tint overworld to give focus to battle
		GameManager.inst.overworld.EnableTint();
		GameManager.inst.overworld.ClearOverlayTiles();
	}
}
