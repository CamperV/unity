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
	// General priorities:
    // 1) in attack range
    // 2) highest damage
    // 3) longest range
    // 4) best chance to hit
    // 5) best chance to crit

	private EnemyUnit thisUnit;
	private List<PlayerUnit> targets;

	// public struct Thoughts {
	// 	bool willIAttackThisTurn = false;
	// 	bool willIDieIfIAttack = false;
	// }
	public struct DamagePackage {
		public PlayerUnit target;
		public GridPosition fromPosition;
		public int potentialDamage;

		public DamagePackage(PlayerUnit t, GridPosition gp, int d) {
			target = t;
			fromPosition = gp;
			potentialDamage = d;
		}
	}

	void Awake() {
		thisUnit = GetComponent<EnemyUnit>();
	}

	public int CalculateInitiative() {
		return 1;
	}

	public void RefreshTargets(List<PlayerUnit> _targets) => targets = _targets;

	// behavior: wants to do the most damage, but is easily distractible by damageable units in its range
	// get the GridPositions where this unit can stand and attack
	// then select the maximum damage one
	public IEnumerable<DamagePackage> OptimalDamagePackages() {

		/////////////////////////////////////////////////////////////////////////////////////////////
		// find the GP that can be occupied by thisUnit, and calculate the optimal damage for each //
		/////////////////////////////////////////////////////////////////////////////////////////////
		List<DamagePackage> damagePackages = new List<DamagePackage>();
		
		// 1) Find all units in AttackRange
		foreach (PlayerUnit potentialTarget in targets.FindAll(t => thisUnit.attackRange.ValidAttack(t.gridPosition))) {

			// 2) Find all positions around each of these targets that can be occupied by thisUnit
			foreach (GridPosition potentialNewPosition in CanPathToThenAttack(potentialTarget.gridPosition)) {

				// 3) Simulate the damage attacking this target from this location
				damagePackages.Add(
					new DamagePackage(
						potentialTarget, 
						potentialNewPosition,
						SimulateDamage(potentialTarget, potentialNewPosition)
					)
				);
			}
		}

		// 4) yield the damage packages in the following order:
		// 		a) highest damage
		//		b) closest target to current position
		//		c) farthest potentialNewPosition from potentialTarget.gridPosition (i.e. an archer maximizing range)
		foreach (DamagePackage dp in damagePackages.OrderByDescending(PotentialDamage)
												   .ThenBy(ClosestPosition)
												   .ThenByDescending(FarthestRange)) {
			yield return dp;
		}
	}

	private IEnumerable<GridPosition> CanPathToThenAttack(GridPosition potentialTargetPosition) {
		foreach (GridPosition withinRange in potentialTargetPosition.Radiate(thisUnit.unitStats.MAX_RANGE, min: thisUnit.unitStats.MIN_RANGE)) {	
			if (thisUnit.moveRange.ValidMoveTo(withinRange)) yield return withinRange;
		}
	}

	private int SimulateDamage(PlayerUnit target, GridPosition fromPosition) {
		return 10;
	}

	private int PotentialDamage(DamagePackage dp) => dp.potentialDamage;
	private int ClosestPosition(DamagePackage dp) => thisUnit.gridPosition.ManhattanDistance(dp.fromPosition);
	private int FarthestRange(DamagePackage dp)   => dp.target.gridPosition.ManhattanDistance(dp.fromPosition);
}



