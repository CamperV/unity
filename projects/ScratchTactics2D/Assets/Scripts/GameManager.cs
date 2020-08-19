using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager inst = null; // enforces singleton behavior
	//
	private WorldGrid worldGrid;
	private Player player;
	
	void Awake() {
		// only allow one GameManager to exist at any time
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		
		// don't kill when reloading a Scene
		DontDestroyOnLoad(gameObject);
		
		worldGrid = GetComponentsInChildren<WorldGrid>()[0];
		player    = GetComponentsInChildren<Player>()[0];
		init();
	}
	
	void init() {
		worldGrid.GenerateWorld();
		
		player.SetWorld(worldGrid);
		player.ResetPosition();
		
		Debug.Log("Found " + SortingLayer.layers + " sorting layers");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
