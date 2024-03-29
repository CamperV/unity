﻿using System.Collections;
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
	public Battle battlePrefab;
	//
	public PlayerArmyController playerArmyControllerPrefab;
	public EnemyArmyController enemyArmyControllerPrefab;
	
	// these are public so the EnemyManager can access Player locations
	[HideInInspector] public Overworld overworld;
	[HideInInspector] public PlayerArmy playerArmy;
	[HideInInspector] public IEnumerable enemyArmies {
		get {
			foreach (EnemyArmy ea in enemyArmyController.activeRegistry) {
				yield return ea;
			}
		}
	}
	//
	[HideInInspector] public PlayerArmyController playerArmyController;
	[HideInInspector] public EnemyArmyController enemyArmyController;
	
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
		
	void Init() {
		if (seed != -1) Random.InitState(seed);
		
		overworld = Instantiate(overworldPrefab, Vector3.zero, Quaternion.identity);
		//
		// these will have to "register" their subjects
		playerArmyController = Instantiate(playerArmyControllerPrefab, overworld.transform);
		enemyArmyController  = Instantiate(enemyArmyControllerPrefab, overworld.transform);

		// generate the world and spawn the player into it
		overworld.GenerateWorld();
		playerArmy = PlayerArmy.Spawn(playerPrefab);
		playerArmyController.Register(playerArmy);
		
		// right now, only Overworld terrain can spawn enemies
		foreach (var spawner in overworld.Terrain.OfType<IEnemyArmySpawner>()) {
			bool _success = spawner.AttemptToSpawnArmy(enemyArmyController);
		}
		Debug.Log($"Spawned {enemyArmyController.registry.Count} enemy armies");
		
		enemyArmyController.SetTraversableTiles();
		enemyArmyController.InitFlowField(playerArmy.gridPosition);
	}

	void Start() {
		StartCoroutine( Utils.LateFrame(StartGame) );
	}

	public void StartGame() {
		foreach (var starter in FindObjectsOfType<MonoBehaviour>().OfType<IFirstFrame>()) {
			starter.OnFirstFrame();
		}
		
		// now, "enable"
		EnterOverworldState();
	}
	
	public void EnterOverworldState() {
		// refit/retrack camera
		Camera.main.GetComponent<CameraManager>().SetTracking(playerArmy.transform);
		
		// show the overworld clearly
		overworld.DisableTint();
		
		gameState = Enum.GameState.overworld;
	}
	
	public void EnterBattleState() {
		// freeze camera
		Camera.main.GetComponent<CameraManager>().SetTracking(Camera.main.transform);
		
		// tint overworld to give focus to battle
		overworld.EnableTint();
		overworld.ClearOverlayTiles();
		
		// give all control to TacticsManager
		gameState = Enum.GameState.battle;
	}
}
