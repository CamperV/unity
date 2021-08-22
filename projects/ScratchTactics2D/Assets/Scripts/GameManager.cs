using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior
	public Enum.GameState gameState { get; set; }

	public int seed = -1;
	
	// prefabs to be instantiated
	public Overworld overworldPrefab;
	public PlayerArmy playerPrefab;
	public PhaseManager phaseManagerPrefab;
	public MouseManager mouseManagerPrefab;
	public TacticsManager tacticsManagerPrefab;
	//
	public PlayerArmyController playerControllerPrefab;
	public EnemyArmyController enemyControllerPrefab;
	
	// these are public so the EnemyManager can access Player locations
	[HideInInspector] public Overworld overworld;
	[HideInInspector] public PlayerArmy player;
	[HideInInspector] public PhaseManager phaseManager;
	[HideInInspector] public MouseManager mouseManager;
	//
	[HideInInspector] public PlayerArmyController playerController;
	[HideInInspector] public EnemyArmyController enemyController;
	
	//
	// this has its own grid/tilemap children
	[HideInInspector] public TacticsManager tacticsManager;
	
	void Awake() {
		// only allow one GameManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		
		//
		Init();
	}
	
	void Start() {
		// now, "enable"
		EnterOverworldState();
	}
	
	void Init() {
		if (seed != -1) Random.InitState(seed);
		
		overworld      = Instantiate(overworldPrefab, Vector3.zero, Quaternion.identity);
		mouseManager   = Instantiate(mouseManagerPrefab);
		phaseManager   = Instantiate(phaseManagerPrefab);
		tacticsManager = Instantiate(tacticsManagerPrefab, Vector3.zero, Quaternion.identity);
		//
		// these will have to "register" their subjects
		playerController = Instantiate(playerControllerPrefab);
		enemyController  = Instantiate(enemyControllerPrefab);

		// generate the world and spawn the player into it
		overworld.GenerateWorld();
		player = PlayerArmy.Spawn(playerPrefab);
		playerController.Register(player);
		
		// right now, only Overworld terrain can spawn enemies
		foreach (var spawner in overworld.Terrain.OfType<IEnemyArmySpawner>()) {
			bool _success = spawner.AttemptToSpawnArmy();
		}
		Debug.Log($"Spawned {enemyController.registry.Count} enemy armies");
		
		enemyController.SetTraversableTiles();
		enemyController.InitFlowField(player.gridPosition);
	}
	
	public void EnterOverworldState() {
		// refit/retrack camera
		Camera.main.GetComponent<CameraManager>().SetTracking(player.transform);
		
		// show the overworld clearly
		overworld.DisableTint();
		
		gameState = Enum.GameState.overworld;
		phaseManager.StartPhase(Enum.Phase.player);
		playerController.TriggerPhase();
	}
	
	public void EnterBattleState() {
		// freeze camera
		Camera.main.GetComponent<CameraManager>().SetTracking(Camera.main.transform);
		
		// tint overworld to give focus to battle
		overworld.EnableTint();
		overworld.ClearOverlayTiles();
		
		// give all control to TacticsManager
		gameState = Enum.GameState.battle;
		phaseManager.StartPhase(Enum.Phase.none);
	}
}
