using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class GameTile : Tile
{
}

public abstract class WorldTile : GameTile
{
	// returns a probability from 0-99 that this tile is generated
	public virtual int probability { get => 0; }
	
	// utility methods for getting specific worldtile sprites
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}

public abstract class TacticsTile : GameTile
{
	// returns an integer that signifies the cost of entering this tile
	public virtual int cost { get => Constants.standardTickCost; }
	public List<Sprite> sprites;
}

public abstract class OverlayTile : Tile
{
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}

public abstract class UITile : Tile 
{
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}