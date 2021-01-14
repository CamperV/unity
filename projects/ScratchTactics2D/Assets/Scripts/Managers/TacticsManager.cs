using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TacticsManager : MonoBehaviour
{
	private Vector3 screenPoint;
	private Vector3 dragOffset;
	private bool draggingView;
	
	public Battle battlePrefab;
	[HideInInspector] public Battle activeBattle;

	public bool scrollLock;

	// only one Unit can be in focus at any given time
	public Unit focusSingleton { get; set; }
	
	void Awake() {
		dragOffset = Vector3.zero;
		draggingView = false;
		scrollLock = false;
	}

    void Update() {
		// while "in-battle" wait for key commands to exit state
		if (GameManager.inst.gameState == Enum.GameState.battle) {
			if (Input.GetKeyDown("space")) {
				Debug.Log("Exiting Battle...");
				
				DestroyActiveBattle();
			}
			
			if (!scrollLock) {
				var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//
				// RMB drag view
				//
				if (Input.GetMouseButtonDown(1) && !draggingView) {
					dragOffset = activeBattle.transform.position - mouseWorldPos;
					draggingView = true;
				}
				// update pos by offset, release drag when mouse goes up
				if (draggingView) {
					activeBattle.transform.position = mouseWorldPos + dragOffset;
				}

				// make sure we can drop out of the dragging mode
				if (draggingView && (!Input.GetMouseButton(1)))
					draggingView = false;
				
				//
				// mouse view control
				//
				if (!draggingView) {
					// calculate what the new scale WILL been
					// and calculate the scale ratio. Just use X, because our scale is uniform on all axes
					var updatedScale = activeBattle.transform.localScale + (Input.GetAxis("Mouse ScrollWheel") * 0.75f) * Vector3.one;
					float scaleRatio = updatedScale.x / activeBattle.transform.localScale.x;
					
					Vector3 localToMouse = activeBattle.transform.position - mouseWorldPos;
					
					//update the scale, and position based on the new scale
					activeBattle.transform.localScale = updatedScale;
					activeBattle.transform.position = mouseWorldPos + (localToMouse * scaleRatio);
				}
			}

			//
			focusSingleton = null;
		}       
    }
	
	public TacticsGrid GetActiveGrid() {
		Debug.Assert(GameManager.inst.gameState == Enum.GameState.battle);
		return activeBattle.grid;
	}
	
	// handles construction of Battle and management of tacticsGrid
	public void CreateActiveBattle(List<OverworldEntity> participants, List<WorldTile> tiles, Enum.Phase initiatingPhase) {
		var cameraPos = new Vector3(Camera.main.transform.position.x,
									Camera.main.transform.position.y,
									0);		
		activeBattle = Instantiate(battlePrefab, cameraPos, Quaternion.identity);
		activeBattle.Init(participants, tiles);
		//
		activeBattle.StartBattleOnPhase(initiatingPhase);
	}

	public void ResolveActiveBattle(OverworldEntity defeatedEntity) {
		defeatedEntity.Die();
		DestroyActiveBattle();
	}
	
	public void DestroyActiveBattle() {
		Destroy(activeBattle.grid.gameObject);
		Destroy(activeBattle.gameObject);
		//
		GameManager.inst.EnterOverworldState();
	}
}
