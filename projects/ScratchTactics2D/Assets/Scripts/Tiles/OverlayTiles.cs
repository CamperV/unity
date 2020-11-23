using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class SelectOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("select_overlay_tile")
		};
		sprite = sprites[0];
	}
	
	public static SelectOverlayTile GetTileWithSprite(int spriteIndex) {
		SelectOverlayTile wt = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class PathOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_dot");
	}
}

public class EndpointOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_end");
	}
}

public class SelectOverlayIsoTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("select_overlay_iso_tile");
	}
}