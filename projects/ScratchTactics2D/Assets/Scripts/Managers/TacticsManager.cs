using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
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
	public Unit focusSingleton { get; private set; }
	
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

					if (scaleRatio != 1.0f) {
						Vector3 localToMouse = activeBattle.transform.position - mouseWorldPos;
						
						//update the scale, and position based on the new scale
						activeBattle.transform.localScale = updatedScale;
						activeBattle.transform.position = mouseWorldPos + (localToMouse * scaleRatio);
					}
				}
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

			//
			// finally, Ghost Control
			var grid = GetActiveGrid();
			var descendingInBattle = activeBattle.GetRegisteredInBattle().OrderByDescending(it => it.gridPosition.y);

			foreach (Unit u in descendingInBattle) {
				u.ghosted = false;
				if (!u.inFocus) {
					// each unit will check its own processes to see if it should be ghosted
					// having multiple senders, i.e. PathOverlayIso tiles and other Units, is difficult to keep track of

					// if there is any overlay that can be obscured:
					Vector3Int northPos = u.gridPosition + new Vector3Int(1, 1, 0);
					if (grid.GetOverlayAt(u.gridPosition) || grid.GetOverlayAt(northPos)) {
						u.ghosted = true;
					}
							
					// or, if there is a Unit with an active focus right behind
					if (((Unit)grid.OccupantAt(northPos))?.inFocus ?? false) {
						u.ghosted = true;
					}
				}
			}
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

	public Unit GetNewFocus() {
		var mm = GameManager.inst.mouseManager;

		// Focus control: reset if applicable and highlight/focus
		var unitsInBattle = activeBattle.GetRegisteredInBattle().OrderBy(it => it.gridPosition.y);
		foreach (Unit u in unitsInBattle) {
			if (!u.ghosted && u.ColliderContains(mm.mouseWorldPos)) {
				return u;
			}
		}
		
		// secondary try: select based on tileGridPos AFTER determining BB collisions
		foreach (Unit u in unitsInBattle) {
			if (!u.IsMoving() && mm.currentMouseGridPos == u.gridPosition) {
				return u;
			}
		}

		return null;
	}
}
