using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	// singleton
	public static GameManager inst = null; // enforces singleton behavior

	// accessed by children via singleton
	[HideInInspector] public bool playerPhase = true;
	public int maxEnemies;
	public int minEnemies;
	
	// prefabs to be instantiated
	public WorldGrid worldGridPrefab;
	public Player playerPrefab;
	public Enemy enemyPrefab;
	
	[HideInInspector] public WorldGrid worldGrid;
	private List<Enemy> enemies;
	
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
		worldGrid = null;
		enemies = new List<Enemy>();
		Init();
	}
	
	void Init() {
		worldGrid = Instantiate(worldGridPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		worldGrid.GenerateWorld();
		
		Player player = Instantiate(playerPrefab, worldGrid.RandomTileReal(), Quaternion.identity);
		player.ResetPosition();
		
		// now, spawn the enemies
		enemies.Clear();
		for (int i = 0; i < Random.Range(minEnemies, maxEnemies); i++) {
			Enemy newEnemy = Instantiate(enemyPrefab, worldGrid.RandomTileReal(), Quaternion.identity);
			enemies.Add(newEnemy);
		}
	}

    // Update is called once per frame
    void Update() {
        
    }
}
