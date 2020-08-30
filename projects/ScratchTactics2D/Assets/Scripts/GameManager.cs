using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior

	// accessed by children via singleton
	public enum Phase {player, enemy};
	private Phase phase;
	
	[HideInInspector] public Phase currentPhase {
		get { return phase; }
		set { phase = value; }
	}
	public int maxEnemies;
	public int minEnemies;
	
	// prefabs to be instantiated
	public WorldGrid worldGridPrefab;
	public Player playerPrefab;
	public Enemy enemyPrefab;
	public EnemyController enemyControllerPrefab;
	
	// these are public so the EnemyController can access Player locations
	[HideInInspector] public WorldGrid worldGrid;
	[HideInInspector] public Player player;
	[HideInInspector] public EnemyController enemyController;
	
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
		worldGrid = Instantiate(worldGridPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		worldGrid.GenerateWorld();
		
		player = Instantiate(playerPrefab, worldGrid.RandomTileReal(), Quaternion.identity);
		player.ResetPosition();
		
		enemyController = Instantiate(enemyControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		
		// now, spawn the enemies
		for (int i = 0; i < Random.Range(minEnemies, maxEnemies); i++) {
			Enemy newEnemy = Instantiate(enemyPrefab, worldGrid.RandomTileReal(), Quaternion.identity);
			enemyController.AddSubject(newEnemy);
		}
		
		// first phase goes to the player
		currentPhase = Phase.player;
	}

    void Update() {
		// wait for phase objects to signal, and change phase for them
		if (currentPhase == Phase.player) {
			//
			// code spins here until player takes its phaseAction
			//
			
			if (player.phaseActionTaken) {
				currentPhase = Phase.enemy;
				enemyController.phaseActionTaken = false;
			}
		}
		else if (currentPhase == Phase.enemy) {
			//
			// code spins here until all enemies takes their phaseAction
			//
			
			if (enemyController.phaseActionTaken) {
				currentPhase = Phase.player;
				player.phaseActionTaken = false;
			}
		}
	}
}
