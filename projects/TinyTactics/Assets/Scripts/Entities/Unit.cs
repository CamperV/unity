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
    protected IUnitPhase unitPhase;
    protected PlayerUnitController playerUnitController;
    protected EnemyUnitController enemyUnitController;

    // other
    public MoveRange moveRange;
    public AttackRange attackRange;

    public bool turnActive { get=> unitPhase.active; }

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        mapPathfinder = GetComponent<UnitPathfinder>();
        unitStats = GetComponent<UnitStats>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponent<UnitMap>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();

        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();
    }

    // we must take care to add certain functions to the MoveRange
    // The MoveRange field.Keys indicate what tiles can be pathed through
    // However, MoveRange doesn't know what tiles it cannot stand on
    // we pass it a UnitAt lambda to tell it you can't validly stand on occupied tiles
    public void UpdateThreatRange(bool standing = false) {
        int movement = (unitPhase.moveAvailable && standing == false) ? unitStats.MOVE : 0;
        moveRange = mapPathfinder.GenerateFlowField<MoveRange>(gridPosition, range: movement);
        moveRange.RegisterValidMoveToFunc(unitMap.CanMoveInto);

        attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);
    }
}
