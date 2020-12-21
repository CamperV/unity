using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class GameTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public virtual int cost { get { return 1; } }
}

public abstract class WorldTile : GameTile
{
	// returns a probability from 0-99 that this tile is generated
	public virtual int probability { get { return 0; } }
	
	// returns the dimensions of the battleGrid to be created when a Battle is started on this tile
	public virtual Vector2Int battleGridSize { get { return new Vector2Int(8, 8); } }

	// reserved for tiles that have visual depth, ie mountains
	public virtual int depth { get { return 0; } }

	// used for determining movement through terrain, and "how long" it takes
	public virtual int tickCost { get { return 100; } }
	
	// utility methods for getting specific worldtile sprites
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}

public abstract class TacticsTile : GameTile
{
	public List<Sprite> sprites;
}

public abstract class OverlayTile : Tile
{
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}