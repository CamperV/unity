using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "CustomAssets/TileVisuals")]
public class TileVisuals : ScriptableObject
{
    public Color color;
    public Color altColor;
    
    public VisualTile tile;
}