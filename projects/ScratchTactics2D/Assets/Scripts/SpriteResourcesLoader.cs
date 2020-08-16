using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesResourcesLoader : MonoBehaviour
{
	public static Sprite getPlayerSprite() {
		return getSprite("yellow_skull_red_eyes");
	}
	
	private static Sprite getSprite(string name) {
		return (Sprite)Resources.Load(name, typeof(Sprite));
	}
}
