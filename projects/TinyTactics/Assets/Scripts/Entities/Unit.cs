using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class Unit : MonoBehaviour, IGridPosition
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }

    // necessary Component references
    protected UnitMap unitMap;
    protected BattleMap battleMap;
    protected SpriteAnimator spriteAnimator;
    protected UnitPathfinder mapPathfinder;
    protected UnitStats unitStats;
    protected UnitPhase unitPhase;

    // other
    protected MoveRange moveRange;
    protected AttackRange attackRange;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        mapPathfinder = GetComponent<UnitPathfinder>();
        unitStats = GetComponent<UnitStats>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponent<UnitMap>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();
    }

    protected MoveRange GenerateMoveRange(GridPosition gp, int range) {
        return mapPathfinder.GenerateFlowField<MoveRange>(gp, range: range);
    }

    protected AttackRange GenerateAttackRange(int minRange, int maxRange) {
        return new AttackRange(moveRange, minRange, maxRange);
    }
}
