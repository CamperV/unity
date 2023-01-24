using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainEffectPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectValue;
    [SerializeField] private Image tileValue;

	public void UpdatePanel(TerrainTile terrainAt) {
		tileValue.sprite = terrainAt.sprite;

		if (terrainAt.HasTerrainEffect) {
			effectValue.SetText($"{terrainAt.displayName}: {terrainAt.terrainEffect.mutatorDisplayData.name}");
		} else {
			effectValue.SetText($"{terrainAt.displayName}");	
		}
	}
}
