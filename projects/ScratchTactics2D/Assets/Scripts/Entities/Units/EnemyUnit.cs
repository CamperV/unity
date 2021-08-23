using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public abstract class EnemyUnit : Unit
{
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
