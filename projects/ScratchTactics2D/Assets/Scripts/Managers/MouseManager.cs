using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class MouseManager : MonoBehaviour
{
	private GameGrid currentActiveGrid;
	
	[HideInInspector] public Vector3 mouseWorldPos;

    void Update() {
		// get the collision point of the ray with the z = 0 plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mouseWorldPos = ray.GetPoint(-ray.origin.z / ray.direction.z);
    }
}