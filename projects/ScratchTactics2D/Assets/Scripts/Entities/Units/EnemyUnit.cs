using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public class EnemyUnit : Unit
{
	private static Dictionary<string, Color> enemyUnitDefaultPalette = new Dictionary<string, Color>{
		// ["_BrightColor"] = Color.magenta,
		// ["_MediumColor"] = Color.Lerp(Color.magenta, Color.red, 0.5f),
		// ["_DarkColor"]   = Color.red,
		// ["_ShadowColor"] = Color.black,
		["_BrightColor"] = new Color(0.97f, 0.55f, 0.55f),
		["_MediumColor"] = new Color(0.87f, 0.41f, 0.41f),
		["_DarkColor"]   = new Color(0.32f, 0.17f, 0.17f),
		["_ShadowColor"] = Color.black,
	};

	public IEnemyUnitClass unitClass;
	public EnemyBrain brain;	// assigned in ApplyState() only

	public override void ApplyState(UnitState state) {
		unitState = state;

		// use the state to add certain Components as well
		unitClass = (gameObject.AddComponent(Type.GetType(unitState.unitClass)) as IEnemyUnitClass);
		brain     = (gameObject.AddComponent(Type.GetType(unitClass.assignedBrain)) as EnemyBrain);

		GetComponent<Animator>().runtimeAnimatorController = (unitClass as UnitClass).unitAnimator;
		GetComponent<PaletteSwapAndOutlineBehavior>().SetPalette(enemyUnitDefaultPalette);

		unitUI.UpdateWeaponEmblem(equippedWeapon);
		unitUI.UpdateHealthBar(_HP);
		unitUI.SetTransparency(0.0f);
	}

	public override void DisplayThreatRange() {
		moveRange?.ClearDisplay(Battle.active.grid);
		attackRange?.ClearDisplay(Battle.active.grid);

		UpdateThreatRange();
		attackRange.Display(Battle.active.grid);
		moveRange.Display(Battle.active.grid, Constants.threatColorYellow);

		// add the lil selection square
		Battle.active.grid.UnderlayAt(gridPosition, Constants.selectColorWhite);
	}
}
