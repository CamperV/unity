﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteResourcesLoader : MonoBehaviour
{
	public static Sprite GetPlayerSprite() {
		return GetSprite("yellow_skull");
	}
	
	public static Sprite GetEnemySprite() {
		return GetSprite("yellow_skull_red_eyes");
	}
	
	public static Sprite GetSprite(string name) {
		return (Sprite)Resources.Load(name, typeof(Sprite));
	}
}
