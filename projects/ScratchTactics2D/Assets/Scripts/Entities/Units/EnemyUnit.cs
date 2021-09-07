using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public class EnemyUnit : Unit
{
	public UnitClass unitClass;

	public override void ApplyState(UnitState state) {
		unitState = state;

		// use the state to add certain Components as well
		UnitClass unitClass = gameObject.AddComponent(Type.GetType(unitState.unitClass)) as UnitClass;

		GetComponent<Animator>().runtimeAnimatorController = unitClass.enemyUnitAnimator;

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
