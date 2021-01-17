using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior
	public Enum.GameState gameState { get; private set; }

	// accessed by children via singleton
	public int maxEnemies;
	public int minEnemies;
	
	// prefabs to be instantiated
	public WorldGrid worldGridPrefab;
	public OverworldPlayer playerPrefab;
	public PhaseManager phaseManagerPrefab;
	public MouseManager mouseManagerPrefab;
	public TacticsManager tacticsManagerPrefab;
	//
	public PlayerController playerControllerPrefab;
	public EnemyController enemyControllerPrefab;
	//
	public OverworldEnemyBase enemyPrefab;
	
	// these are public so the EnemyManager can access Player locations
	[HideInInspector] public WorldGrid worldGrid;
	[HideInInspector] public OverworldPlayer player;
	[HideInInspector] public PhaseManager phaseManager;
	[HideInInspector] public MouseManager mouseManager;
	//
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public EnemyController enemyController;
	
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
		// initial phase
		phaseManager.StartPhase(Enum.Phase.player);
	}
	
	void Init() {
		worldGrid      = Instantiate(worldGridPrefab, Vector3.zero, Quaternion.identity);
		mouseManager   = Instantiate(mouseManagerPrefab);
		phaseManager   = Instantiate(phaseManagerPrefab);
		tacticsManager = Instantiate(tacticsManagerPrefab, Vector3.zero, Quaternion.identity);
		//
		// these will have to "register" their subjects
		playerController = Instantiate(playerControllerPrefab);
		enemyController  = Instantiate(enemyControllerPrefab);

		// generate the world and spawn the player into it
		worldGrid.GenerateWorld();
		player = OverworldPlayer.Spawn(playerPrefab);
		playerController.Register(player);
		
		// now, spawn the enemies
		for (int i = 0; i < Random.Range(minEnemies, maxEnemies); i++) {
			var enemy = OverworldEnemyBase.Spawn(enemyPrefab);
			enemyController.Register(enemy);
		}
		
		enemyController.SetTraversableTiles();
		enemyController.InitFlowField(player.gridPosition);
		
		// now, "enable"
		EnterOverworldState();
	}
	
	public void EnterOverworldState() {
		// refit/retrack camera
		CameraManager.SetTracking(player.transform);
		
		// show the overworld clearly
		worldGrid.DisableTint();
		
		gameState = Enum.GameState.overworld;
	}
	
	public void EnterBattleState() {
		// freeze camera
		CameraManager.SetTracking(Camera.main.transform);
		
		// tint overworld to give focus to battle
		worldGrid.EnableTint();
		worldGrid.ClearOverlayTiles();
		
		// give all control to TacticsManager
		gameState = Enum.GameState.battle;
		phaseManager.StartPhase(Enum.Phase.none);
	}

	// convenience
	public GameGrid GetActiveGrid() {
		switch (gameState) {
			case Enum.GameState.overworld:
				return worldGrid;
			case Enum.GameState.battle:
				return tacticsManager.GetActiveGrid();
			default:
				return worldGrid;
		}
	}
}
