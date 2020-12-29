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
        UpdateBar(max);

        barTilemap.CompressBounds();
		barTilemap.RefreshAllTiles();
    }

    public void UpdateBar(int val) {
        // fully draw here
        for (int i = 0; i < maxPips; i++) {
            int x = i % pipsInRow;
            int y = (int)(i / pipsInRow); // this should truncate, do you C#?
            
            barTilemap.SetTile(new Vector3Int(x, y, 0), (i < val) ? fullPip : emptyPip);
        }
    }
}
