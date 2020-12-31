using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HealthBar : MonoBehaviour
{
    public int maxPips;
    public int pipsInRow;

    [HideInInspector] public HealthPipTile fullPip;
    [HideInInspector] public HealthPipTile emptyPip;
	[HideInInspector] public Tilemap barTilemap;
	
	void Awake() {
        fullPip  = (ScriptableObject.CreateInstance<HealthPipTile>() as HealthPipTile);
        fullPip.SetSprite(0);

        emptyPip = (ScriptableObject.CreateInstance<HealthPipTile>() as HealthPipTile);
        emptyPip.SetSprite(1);

		barTilemap = GetComponentsInChildren<Tilemap>()[0];
    }

    void Start() {
        transform.position -= barTilemap.localBounds.center;
        transform.position -= new Vector3(0, barTilemap.localBounds.center.y*3, 0);
    }

    public void InitHealthBar(int max) {
        maxPips = max;
        UpdateBar(max, 0.0f);

        barTilemap.CompressBounds();
		barTilemap.RefreshAllTiles();
    }

    public void UpdateBar(int val, float alpha) {
        // fully draw here
        for (int i = 0; i < maxPips; i++) {
            int x = i % pipsInRow;
            int y = (int)(i / pipsInRow); // this should truncate, do you C#?
            
            Vector3Int tilePos = new Vector3Int(x, y, 0);
            barTilemap.SetTile(tilePos, (i < val) ? fullPip : emptyPip);
            //
            var currColor = barTilemap.GetColor(tilePos);
            currColor.a = alpha;
            barTilemap.SetTileFlags(tilePos, TileFlags.None);
            barTilemap.SetColor(tilePos, currColor);
        }
    }

    public void UpdateBarTransparency(float alpha) {
        // fully draw here
        for (int i = 0; i < maxPips; i++) {
            int x = i % pipsInRow;
            int y = (int)(i / pipsInRow); // this should truncate, do you C#?
            
            Vector3Int tilePos = new Vector3Int(x, y, 0);
            //
            var currColor = barTilemap.GetColor(tilePos);
            currColor.a = alpha;
            barTilemap.SetTileFlags(tilePos, TileFlags.None);
            barTilemap.SetColor(tilePos, currColor);
        }
    }
}
