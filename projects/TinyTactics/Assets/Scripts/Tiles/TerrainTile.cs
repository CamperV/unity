using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public abstract class TerrainTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	[SerializeField] public int cost;
}