using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	private Vector3Int currentMouseGridPos;
	private WorldGrid worldGridInst;
	private SelectOverlayTile selectOverlayTile;
	
	// dont' use Awake here, to avoid bootstrapping issues
    void Start() {
		currentMouseGridPos = Vector3Int.zero;
		worldGridInst = GameManager.inst.worldGrid;
		selectOverlayTile = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile;
    }

    void Update() {
		// remove previous highlighting
		worldGridInst.ResetOverlayAt(currentMouseGridPos, selectOverlayTile.level);
		
		// get new position
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currentMouseGridPos = worldGridInst.Real2GridPos(mouseWorldPos);
		
		worldGridInst.OverlayAt(currentMouseGridPos, selectOverlayTile);
    }
}