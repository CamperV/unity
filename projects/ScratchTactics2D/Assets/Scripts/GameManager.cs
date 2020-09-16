using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior

	// accessed by children via singleton
	public int maxEnemies;
	public int minEnemies;
	
	// prefabs to be instantiated
	public WorldGrid worldGridPrefab;
	public Player playerPrefab;
	public Enemy enemyPrefab;
	public EnemyManager enemyManagerPrefab;
	public PhaseManager phaseManagerPrefab;
	public MouseManager mouseManagerPrefab;
	
	// these are public so the EnemyManager can access Player locations
	[HideInInspector] public WorldGrid worldGrid;
	[HideInInspector] public Player player;
	[HideInInspector] public EnemyManager enemyManager;
	[HideInInspector] public PhaseManager phaseManager;
	[HideInInspector] public MouseManager mouseManager;
	
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
		worldGrid    = Instantiate(worldGridPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		mouseManager = Instantiate(mouseManagerPrefab);
		phaseManager = Instantiate(phaseManagerPrefab);
		enemyManager = Instantiate(enemyManagerPrefab);
		
		// generate the world and spawn the player into it
		worldGrid.GenerateWorld();
		player = Player.Spawn(playerPrefab);
		
		// now, spawn the enemies
		for (int i = 0; i < Random.Range(minEnemies, maxEnemies); i++) {
			Enemy newEnemy = Enemy.Spawn(enemyPrefab);
			enemyManager.AddSubject(newEnemy);
		}
	}
}
