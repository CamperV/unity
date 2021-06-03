using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceLoader : MonoBehaviour
{
	public static Sprite GetSprite(string name) {
		var retval = (Sprite)Resources.Load(name, typeof(Sprite));
		if (retval == null) {
			Debug.LogException(new Exception($"Loading of resource {name} failed"));
		}
		return retval;
	}
	
	public static Sprite GetMultiSprite(string multiName, string name) {
		Sprite[] sprites = Resources.LoadAll<Sprite>(multiName);
		return sprites.Single(s => s.name == name);
	}
}
