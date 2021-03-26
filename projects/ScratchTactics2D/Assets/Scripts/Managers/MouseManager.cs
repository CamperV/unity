using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class MouseManager : MonoBehaviour
{
	private GameGrid currentActiveGrid;
	
	[HideInInspector] public Vector3 mouseWorldPos;
	[HideInInspector] public Vector3Int prevMouseGridPos = Vector3Int.zero;

    void Update() {
		// this ScreenToWorldPoint ignores the depth of the camera. Is this intended vis-a-vis Unity, or is this a cheap shortcut?
		//mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -1 * Camera.main.transform.position.z));

		// get the collision point of the ray with the z = 0 plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mouseWorldPos = ray.GetPoint(-ray.origin.z / ray.direction.z);
		currentActiveGrid = GameManager.inst.GetActiveGrid();
		
		// debug
		/*
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log("worldPos: " + mouseWorldPos);

			Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 zPoint = myRay.GetPoint(-myRay.origin.z / myRay.direction.z);

			Debug.Log($"Calculating");
			Vector3 invOrigin = new Vector3(myRay.origin.x, myRay.origin.y, -myRay.origin.z);
			foreach (var s in invOrigin.SteppedInterpInc(zPoint, 25)) {
				var cmgp = currentActiveGrid.Real2GridPos(s);
				Debug.Log($"next step [{myRay.origin}->{zPoint}]: {s}, cmgp: {cmgp}");
			}
		}
		*/
    }

	public Vector3Int GetValidIsometricGridPos(TacticsGrid grid) {
		Ray zRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 zPoint = zRay.GetPoint(-zRay.origin.z / zRay.direction.z);
		Vector3 invOrigin = new Vector3(zRay.origin.x, zRay.origin.y, -zRay.origin.z);

		foreach (var s in invOrigin.SteppedInterpInc(zPoint, 25)) {
			Vector3Int currentGridPos = currentActiveGrid.Real2GridPos(s);
			if (grid.IsInBounds(currentGridPos)) {
				prevMouseGridPos = currentGridPos;
				return currentGridPos;
			}
		}
		return prevMouseGridPos;
	}
}