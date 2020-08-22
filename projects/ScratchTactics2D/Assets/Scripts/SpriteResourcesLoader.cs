using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesResourcesLoader : MonoBehaviour
{
	public static Sprite GetPlayerSprite() {
		return GetSprite("yellow_skull_red_eyes");
	}
	
	public static Sprite GetEnemySprite() {
		return GetSprite("yellow_skull");
	}
	
	private static Sprite GetSprite(string name) {
		return (Sprite)Resources.Load(name, typeof(Sprite));
	}
}
