using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceLoader : MonoBehaviour
{
	public static Sprite GetSprite(string name) {
		return (Sprite)Resources.Load(name, typeof(Sprite));
	}
	
	private static Tile GetTile(string name) {
		return (Tile)Resources.Load(name, typeof(Tile));
	}
}
