using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior
	private Enum.GameState gameState;

	// accessed by children via singleton
	public int maxEnemies;
	public int minEnemies;
	
	// prefabs to be instantiated
	public WorldGrid worldGridPrefab;
	public Player playerPrefab;
	public Enemy enemyPrefab;
	public UIManager UIManagerPrefab;
	public EnemyManager enemyManagerPrefab;
	public PhaseManager phaseManagerPrefab;
	public MouseManager mouseManagerPrefab;
	//
	public TacticsManager tacticsManagerPrefab;
	
	// these are public so the EnemyManager can access Player locations
	[HideInInspector] public WorldGrid worldGrid;
	[HideInInspector] public Player player;
	[HideInInspector] public UIManager UIManager;
	[HideInInspector] public EnemyManager enemyManager;
	[HideInInspector] public PhaseManager phaseManager;
	[HideInInspector] public MouseManager mouseManager;
	
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
		worldGrid      = Instantiate(worldGridPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		UIManager      = Instantiate(UIManagerPrefab);
		mouseManager   = Instantiate(mouseManagerPrefab);
		phaseManager   = Instantiate(phaseManagerPrefab);
		enemyManager   = Instantiate(enemyManagerPrefab);
		tacticsManager = Instantiate(tacticsManagerPrefab);
		
		// generate the world and spawn the player into it
		worldGrid.GenerateWorld();
		player = Player.Spawn(playerPrefab);
		
		// now, spawn the enemies
		for (int i = 0; i < Random.Range(minEnemies, maxEnemies); i++) {
			Enemy newEnemy = Enemy.Spawn(enemyPrefab, Enemy.untraversable);
			enemyManager.AddSubject(newEnemy);
		}
		
		enemyManager.SetTraversableTiles();
		enemyManager.InitFlowField(player.gridPosition);
		
		// refit/retrack camera
		CameraManager.SetTracking(player.transform);
		
		// now, "enable"
		EnterOverworldState();
	}
	
	public void EnterOverworldState() {
		gameState = Enum.GameState.overworld;
	}
	
	public void EnterBattleState() {
		gameState = Enum.GameState.battle;
		
		// also, dim the background, enable Tilemaps/etc
		//worldGrid.gameObject.SetActive(false);
	}
}
