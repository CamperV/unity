using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UnitInspector_EnemyRange : MonoBehaviour, IUnitInspector
{
	[SerializeField] private BattleMap battleMap;
    [SerializeField] private TileVisuals moveTileVisuals;
    [SerializeField] private TileVisuals attackTileVisuals;

	void OnDisable() {
		Disable();
	}

	public void ConditionallyInspectUnit(Unit unit) {
		if (unit != null && unit.GetType() != typeof(EnemyUnit)) {
			gameObject.SetActive(false);
		} else {
			InspectUnit(unit);
		}
	}

	public void InspectUnit(Unit unit) {
		if (unit == null) {
			Disable();
			return;
		}

		// else
        unit.personalAudioFX.PlayWakeUpFX();
		unit.UpdateThreatRange();

		unit.attackRange.Display(battleMap, attackTileVisuals.color, attackTileVisuals.tile);
        unit.moveRange.Display(battleMap, moveTileVisuals.color, moveTileVisuals.tile);
        
        battleMap.Highlight(unit.gridPosition, Palette.selectColorWhite);
	}

	private void Disable() {
		battleMap.ResetHighlightTiles();
		battleMap.ResetHighlight();
	}
}