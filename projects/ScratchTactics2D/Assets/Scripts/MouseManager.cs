using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	private WorldGrid worldGridInst;
	private SelectOverlayTile selectOverlayTile;
	
	[HideInInspector] public Vector3Int prevMouseGridPos;
	[HideInInspector] public Vector3Int currentMouseGridPos;
	
	// dont' use Awake here, to avoid bootstrapping issues
    void Start() {
		currentMouseGridPos = Vector3Int.zero;
		worldGridInst = GameManager.inst.worldGrid;
		selectOverlayTile = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile;
    }

    void Update() {
		// store old position and get new position
		prevMouseGridPos = currentMouseGridPos;
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currentMouseGridPos = worldGridInst.Real2GridPos(mouseWorldPos);
		
		//
		// overlay selection tile
		//
		// remove previous highlighting
		if (HasMouseMoved()) {
			worldGridInst.ResetOverlayAt(prevMouseGridPos, selectOverlayTile.level);
			worldGridInst.OverlayAt(currentMouseGridPos, selectOverlayTile);
		}
    }
	
	public bool HasMouseMoved() {
		return worldGridInst.IsInBounds(currentMouseGridPos) && prevMouseGridPos != currentMouseGridPos;
	}
}