using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class ResourceLoader : MonoBehaviour
{
	public static Sprite GetSprite(string name) {
		return (Sprite)Resources.Load(name, typeof(Sprite));
	}
	
	public static Sprite GetMultiSprite(string multiName, string name) {
		Sprite[] sprites = Resources.LoadAll<Sprite>(multiName);
		return sprites.Single(s => s.name == name);
	}
	
	private static Tile GetTile(string name) {
		return (Tile)Resources.Load(name, typeof(Tile));
	}
}
