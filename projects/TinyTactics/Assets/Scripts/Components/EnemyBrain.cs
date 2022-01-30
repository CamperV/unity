using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(EnemyUnit))]
public class EnemyBrain : MonoBehaviour
{
	private EnemyUnit thisUnit;
	private List<PlayerUnit> targets;

	public struct DamagePackage {
		public PlayerUnit target;
		public GridPosition fromPosition;
		public int potentialDamage;
		public bool executableThisTurn;

		public DamagePackage(PlayerUnit t, GridPosition gp, int d, bool execFlag) {
			target = t;
			fromPosition = gp;
			potentialDamage = d;
			executableThisTurn = execFlag;
		}

		public string ToString() => $"DP[{target}, {fromPosition}, {potentialDamage} dmg, {executableThisTurn}]";
	}

	void Awake() {
		thisUnit = GetComponent<EnemyUnit>();
	}

	public void RefreshTargets(List<PlayerUnit> _targets) => targets = _targets;

	// get the smallest distance between you and any of the opposition
	// then, if you can attack that target, get a boost
	// if you can't... fall behind
	// this is a heuristic designed to avoid pathfinding per each target
	public int CalculateInitiative() {
		int minDistToTarget = Int32.MaxValue;
		PlayerUnit closestTarget = targets[0];

		foreach (PlayerUnit target in targets) {
			int dist = thisUnit.gridPosition.ManhattanDistance(target.gridPosition);

			if (dist < minDistToTarget) {
				minDistToTarget = dist;
				closestTarget = target;
			}
		}

		if (thisUnit.attackRange.ValidAttack(closestTarget.gridPosition) == false) minDistToTarget += 100;
		return minDistToTarget;
	}

	// behavior: wants to do the most damage, but is easily distractible by damageable units in its range
	// get the GridPositions where this unit can stand and attack
	// then select the maximum damage one
	public IEnumerable<DamagePackage> OptimalDamagePackagesInRange(MoveRange usingMoveRange, AttackRange usingAttackRange) {
		bool executableThisTurn = usingMoveRange == thisUnit.moveRange && usingAttackRange == thisUnit.attackRange;

		/////////////////////////////////////////////////////////////////////////////////////////////
		// find the GP that can be occupied by thisUnit, and calculate the optimal damage for each //
		/////////////////////////////////////////////////////////////////////////////////////////////
		List<DamagePackage> damagePackages = new List<DamagePackage>();
		
		// 1) Find all units in AttackRange
		foreach (PlayerUnit potentialTarget in targets.FindAll(t => usingAttackRange.ValidAttack(t.gridPosition))) {

			// 2) Find all positions around each of these targets that can be occupied by thisUnit
			foreach (GridPosition potentialNewPosition in CanPathToThenAttack(potentialTarget.gridPosition, usingMoveRange)) {

				// 3) Simulate the damage attacking this target from this location
				damagePackages.Add(
					new DamagePackage(
						potentialTarget, 
						potentialNewPosition,
						SimulateDamageInflicted(potentialTarget, potentialNewPosition),
						executableThisTurn // this dmgPkg can or cannot be executed this turn
					)
				);
			}
		}

		// 4) yield the damage packages in the following order:
		// 		a) highest damage
		//		b) closest target to current position
		//		c) farthest potentialNewPosition from potentialTarget.gridPosition (i.e. an archer maximizing range)
		foreach (DamagePackage dp in damagePackages.OrderBy(CounterAttackPossible)
												   .ThenByDescending(PotentialDamage)
												   .ThenBy(ClosestPosition)
												   .ThenByDescending(FarthestRange)) {
			yield return dp;
		}
	}

	// using a different range. Use this when you want to find an answer outside of thisUnit's range
	private IEnumerable<GridPosition> CanPathToThenAttack(GridPosition potentialTargetPosition, MoveRange usingRange) {
		foreach (GridPosition withinRange in potentialTargetPosition.Radiate(thisUnit.equippedWeapon.weaponStats.MAX_RANGE, min: thisUnit.equippedWeapon.weaponStats.MIN_RANGE)) {	
			if (usingRange.ValidMoveTo(withinRange))
				yield return withinRange;
		}
	}

	private int SimulateDamageInflicted(PlayerUnit target, GridPosition fromPosition) {
		// NOTE: I hate that I'm doing this, but change thisUnit's gridPosition for the simulation
		// THEN CHANGE IT BACK
		GridPosition savedGridPosition = thisUnit.gridPosition;

		thisUnit.gridPosition = fromPosition;
		Engagement potentialEngagement = Engagement.Create(thisUnit, target);
		EngagementStats finalStats = potentialEngagement.SimulateAttack();

		thisUnit.gridPosition = savedGridPosition;
		Debug.Assert(thisUnit.gridPosition == savedGridPosition);

		return finalStats.damage;
	}

	private bool CounterAttackPossible(DamagePackage dp) => Engagement.CounterAttackPossible(thisUnit, dp.target, dp.fromPosition);
	private int  PotentialDamage(DamagePackage dp)       => dp.potentialDamage;
	private int  ClosestPosition(DamagePackage dp) 		 => thisUnit.gridPosition.ManhattanDistance(dp.fromPosition);
	private int  FarthestRange(DamagePackage dp)   		 => dp.target.gridPosition.ManhattanDistance(dp.fromPosition);
}



