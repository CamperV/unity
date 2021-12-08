using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class Unit : MonoBehaviour, IGridPosition
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }
    protected GridPosition _reservedGridPosition; // this is for maintaining state while animating/moving

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

    // we must take care to add certain functions to the MoveRange
    // The MoveRange field.Keys indicate what tiles can be pathed through
    // However, MoveRange doesn't know what tiles it cannot stand on
    // we pass it a UnitAt lambda to tell it you can't validly stand on occupied tiles
    protected MoveRange GenerateMoveRange(GridPosition gp, int range) {
        MoveRange _moveRange = mapPathfinder.GenerateFlowField<MoveRange>(gp, range: range);
        _moveRange.RegisterValidMoveToFunc(gp => unitMap.UnitAt(gp) == null);
        return _moveRange;
    }

    protected AttackRange GenerateAttackRange(int minRange, int maxRange) {
        return new AttackRange(moveRange, minRange, maxRange);
    }
}
