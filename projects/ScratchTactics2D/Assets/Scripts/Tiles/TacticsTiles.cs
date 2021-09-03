using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public abstract class TacticsTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public virtual int cost { get => 1; }

	// does this tile need to be ghosted, i.e. dimmed when actors are behind it?
	public virtual bool dimmable { get => false; }

	// in the same vein: how many tiles does it obscure?
	public virtual float zHeight { get => 0; }
}
//-------------------------------------------------------------------//