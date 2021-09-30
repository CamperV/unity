using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public class EnemyUnit : Unit
{
	public Color color_0 = new Color(0.97f, 0.55f, 0.55f);
	public Color color_1 = new Color(0.87f, 0.41f, 0.41f);
	public Color color_2 = new Color(0.32f, 0.17f, 0.17f);
	public Color color_3 = Color.black;
	private static Dictionary<string, Color> enemyUnitDefaultPalette;

	public IEnemyUnitClass unitClass;
	public EnemyBrain brain;	// assigned in ApplyState() only

	protected override void Awake() {
		base.Awake();

		enemyUnitDefaultPalette = new Dictionary<string, Color>{
			["_BrightColor"] = color_0,
			["_MediumColor"] = color_1,
			["_DarkColor"]   = color_2,
			["_ShadowColor"] = color_3
		};
	}

	public override void ApplyState(UnitState state) {
		unitState = state;

		// use the state to add certain Components as well
		unitClass = (gameObject.AddComponent(Type.GetType(unitState.unitClass)) as IEnemyUnitClass);
		brain     = (gameObject.AddComponent(Type.GetType(unitClass.assignedBrain)) as EnemyBrain);

		GetComponent<Animator>().runtimeAnimatorController = (unitClass as UnitClass).unitAnimator;
		GetComponent<PaletteSwapAndOutlineBehavior>().SetPalette(enemyUnitDefaultPalette);

		// event triggers
		TriggerUpdateEvents();
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
