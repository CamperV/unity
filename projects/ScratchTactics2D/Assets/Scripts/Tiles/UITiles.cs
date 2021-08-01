using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//--------------------------------//
public abstract class UITile : Tile 
{
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}
//--------------------------------//

public class HealthPipTile : UITile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("full_pip"),
            ResourceLoader.GetSprite("empty_pip")
		};
		sprite = sprites[0];
	}
}