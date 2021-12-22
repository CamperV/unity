using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class Unit : MonoBehaviour, IGridPosition, IUnitPhaseInfo
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }
    protected GridPosition _reservedGridPosition; // this is for maintaining state while animating/moving
    protected GridPosition _startingGridPosition; // this is for maintaining a revertable state when prevewing Engagements, etc

    // these are used so that our various Perks can modify the mutable Attack/Defenses that are created during an Engagement
    public delegate void AttackGeneration(ref MutableAttack mutAtt, Unit target);
    public event AttackGeneration OnAttack;

    public delegate void DefenseGeneration(ref MutableDefense mutDef, Unit attacker);
    public event DefenseGeneration OnDefend;
    //

    // necessary Component references
    [HideInInspector] protected UnitMap unitMap;
    [HideInInspector] protected BattleMap battleMap;
    [HideInInspector] public SpriteAnimator spriteAnimator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] protected UnitPathfinder mapPathfinder;
    [HideInInspector] public UnitStats unitStats;
    [HideInInspector] protected HoldTimer holdTimer;
    
    // I don't love this, but it makes things much cleaner.
    protected PlayerUnitController playerUnitController;
    protected EnemyUnitController enemyUnitController;

    // other
    public MoveRange moveRange;
    public AttackRange attackRange;

    // abstract
    public abstract void RevertTurn();
    protected abstract void DisplayThreatRange();
    protected abstract void DisableFSM();

    // IUnitPhaseInfo
    [field: SerializeField] public bool turnActive { get; set; } = true;
    [field: SerializeField] public bool moveAvailable { get; set; } = true;
    [field: SerializeField] public bool attackAvailable { get; set; } = true;
    //
    protected Color originalColor = Color.magenta; // aka no texture, lol

    public bool MouseHovering { get => battleMap.CurrentMouseGridPosition == gridPosition; }

    protected virtual void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapPathfinder = GetComponent<UnitPathfinder>();
        unitStats = GetComponent<UnitStats>();
        holdTimer = GetComponent<HoldTimer>();

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
        int movement = (moveAvailable && standing == false) ? unitStats.MOVE : 0;
        moveRange = mapPathfinder.GenerateFlowField<MoveRange>(gridPosition, range: movement);
        moveRange.RegisterValidMoveToFunc(unitMap.CanMoveInto);

        if (attackAvailable) {
            attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);
        } else {
            attackRange = AttackRange.Empty;
        }
    }

    public void RevertColor() {
        spriteRenderer.color = originalColor;
    }

    public void LerpInactiveColor(float lerpValue) {
        spriteRenderer.color = Color.Lerp(originalColor, new Color(0.75f, 0.75f, 0.75f, 1f), lerpValue);
    }

    // IUnitPhaseInfo
    public void RefreshInfo() {
        turnActive = true;
        moveAvailable = true;
        attackAvailable = true;
        RevertColor();  // to original
        UpdateThreatRange();

        // finally, store your starting location
        // this is relevant for RevertTurn calls
        _startingGridPosition = gridPosition;
    }

    // IUnitPhaseInfo
    public virtual void FinishTurn() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
    }

	public bool SufferDamage(int incomingDamage) {
        unitStats.UpdateHP(unitStats._CURRENT_HP - incomingDamage, unitStats.VITALITY);
		bool survived = unitStats._CURRENT_HP > 0;

        if (!survived) {
            TriggerDeathAnimation();
            DeathCleanUp();
        }

        return survived;
	}

    public void TriggerAttackAnimation(GridPosition towards) {
        StartCoroutine(
            spriteAnimator.BumpTowards<GridPosition>(towards, battleMap, distanceScale: 7.0f)
        );
    }

    public void TriggerHurtAnimation(bool isCritical = false) {
		StartCoroutine( spriteAnimator.FlashColor(Constants.threatColorRed) );
		StartCoroutine( spriteAnimator.Shake((isCritical) ? 0.15f : 0.075f) );
    }

    public void TriggerMissAnimation() {
		StartCoroutine( spriteAnimator.FlashColor(Constants.selectColorWhite) );
        StartCoroutine( spriteAnimator.SmoothBumpRandom(0.10f) );
    }

	private void TriggerDeathAnimation() {
        // note that this will probably start during the "taking damage" animation
		StartCoroutine( spriteAnimator.FadeDownAll(1.0f) );
	}

	private void DeathCleanUp() {
    	StartCoroutine( spriteAnimator.ExecuteAfterAnimating(() => {
            gameObject.SetActive(false);
            DisableFSM();
            unitMap.ClearPosition(gridPosition);
		}));
	}

    public void FireOnAttackEvent(ref MutableAttack mutAtt, Unit target) => OnAttack?.Invoke(ref mutAtt, target);
    public void FireOnDefendEvent(ref MutableDefense mutDef, Unit attacker) => OnDefend?.Invoke(ref mutDef, attacker);
}
