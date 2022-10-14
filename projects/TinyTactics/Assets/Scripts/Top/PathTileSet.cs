using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Extensions;

[CreateAssetMenu (menuName = "TileSets/PathTileSet")]
public class PathTileSet : ScriptableObject
{
    public Sprite Straight_0_CW;
    public Sprite Straight_90_CW;
    
    public Sprite Bend_0_CW;
    public Sprite Bend_90_CW;
    public Sprite Bend_180_CW;
    public Sprite Bend_270_CW;
    
    public Sprite Start_0_CW;
    public Sprite Start_90_CW;
    public Sprite Start_180_CW;
    public Sprite Start_270_CW;

    public Sprite End_0_CW;
    public Sprite End_90_CW;
    public Sprite End_180_CW;
    public Sprite End_270_CW;

    // directional patterns encoded like a clock face
    private HashSet<GridPosition> twelve_six => new HashSet<GridPosition>{ GridPosition.up, GridPosition.down };
    private HashSet<GridPosition> three_nine => new HashSet<GridPosition>{ GridPosition.left, GridPosition.right };
    //
    private HashSet<GridPosition> twelve_three => new HashSet<GridPosition>{ GridPosition.up, GridPosition.right };
    private HashSet<GridPosition> three_six => new HashSet<GridPosition>{ GridPosition.right, GridPosition.down };
    private HashSet<GridPosition> six_nine => new HashSet<GridPosition>{ GridPosition.down, GridPosition.left };
    private HashSet<GridPosition> nine_twelve => new HashSet<GridPosition>{ GridPosition.left, GridPosition.up };
    //
    private HashSet<GridPosition> twelve => new HashSet<GridPosition>{ GridPosition.up };
    private HashSet<GridPosition> three => new HashSet<GridPosition>{ GridPosition.right };
    private HashSet<GridPosition> six => new HashSet<GridPosition>{ GridPosition.down };
    private HashSet<GridPosition> nine => new HashSet<GridPosition>{ GridPosition.left };


    private Tile GetVariant(LinkedListNode<GridPosition> node) {
        Tile _WithSprite(Sprite sprite) {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            return tile;
        }

        // store as a differential
        HashSet<GridPosition> neighborPositions = new HashSet<GridPosition>();
        if (node.Previous != null) neighborPositions.Add(node.Previous.Value - node.Value);
        if (node.Next != null) neighborPositions.Add(node.Next.Value - node.Value);

        // Start
        if (node.Previous == null) {
            if (neighborPositions.SetEquals(twelve)) return _WithSprite(Start_0_CW);
            else if (neighborPositions.SetEquals(three)) return _WithSprite(Start_90_CW);
            else if (neighborPositions.SetEquals(six)) return _WithSprite(Start_180_CW);
            else if (neighborPositions.SetEquals(nine)) return _WithSprite(Start_270_CW);
            else {
                return null;
            }
        }

        // End
        else if (node.Next == null) {
            if (neighborPositions.SetEquals(twelve)) return _WithSprite(End_0_CW);
            else if (neighborPositions.SetEquals(three)) return _WithSprite(End_90_CW);
            else if (neighborPositions.SetEquals(six)) return _WithSprite(End_180_CW);
            else if (neighborPositions.SetEquals(nine)) return _WithSprite(End_270_CW);
            else {
                return null;
            }
        }

        // Segment
        else {
            if (neighborPositions.SetEquals(twelve_six)) return _WithSprite(Straight_0_CW);
            else if (neighborPositions.SetEquals(three_nine)) return _WithSprite(Straight_90_CW);
            else if (neighborPositions.SetEquals(twelve_three)) return _WithSprite(Bend_0_CW);
            else if (neighborPositions.SetEquals(three_six)) return _WithSprite(Bend_90_CW);
            else if (neighborPositions.SetEquals(six_nine)) return _WithSprite(Bend_180_CW);
            else if (neighborPositions.SetEquals(nine_twelve)) return _WithSprite(Bend_270_CW);
            else {
                return null;
            }
        }
    }

	public void DisplayPath(Tilemap tilemap, Path<GridPosition> path) {
		foreach (LinkedListNode<GridPosition> node in path.UnwindNodes()) {
			tilemap.SetTile(
                new Vector3Int(node.Value.x, node.Value.y, -1),
                GetVariant(node)
            );
		}

        // tilemap.SetTile(
        //     new Vector3Int(path.End.x, path.End.y, -1),
        //     GetVariant(path.Last)
        // );
	}

	public void DisplayPath(Tilemap tilemap, Path<GridPosition> path, List<GridPosition> waypoints) {
		foreach (LinkedListNode<GridPosition> node in path.UnwindNodes()) {
			tilemap.SetTile(
                new Vector3Int(node.Value.x, node.Value.y, -1),
                GetVariant(node)
            );
		}

        // foreach (GridPosition wp in waypoints) {
		// 	tilemap.SetTile(
        //         new Vector3Int(wp.x, wp.y, -1),
        //         pathWaypointOverlayTile
        //     );       
        // }

        // tilemap.SetTile(
        //     new Vector3Int(path.End.x, path.End.y, -1),
        //     GetVariant(node)
        // );
	}

    // this uses Vector3Int, to reach that special forbidden zone where GridPosition's cannot reach normally
	public void ClearDisplayPath(Tilemap tilemap) {
        tilemap.CompressBounds();

        foreach (Vector3Int v in tilemap.cellBounds.allPositionsWithin) {
            if (v.z == -1) tilemap.SetTile(v, null);
        }
	}
}
