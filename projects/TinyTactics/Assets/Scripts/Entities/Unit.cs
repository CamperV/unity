using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class Unit : MonoBehaviour, IGridPosition, IUnitPhaseInfo
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }
    protected GridPosition _reservedGridPosition; // this is for maintaining state while animating/moving

    // necessary Component references
    protected UnitMap unitMap;
    protected BattleMap battleMap;
    protected SpriteAnimator spriteAnimator;
    protected SpriteRenderer spriteRenderer;
    protected UnitPathfinder mapPathfinder;
    [HideInInspector] public UnitStats unitStats;
    
    // I don't love this, but it makes things much cleaner.
    protected PlayerUnitController playerUnitController;
    protected EnemyUnitController enemyUnitController;

    // other
    public MoveRange moveRange;
    public AttackRange attackRange;

    // abstract
    public abstract void Cancel();
    protected abstract void DisplayThreatRange();

    // IUnitPhaseInfo
    [field: SerializeField] public bool turnActive { get; set; } = true;
    [field: SerializeField] public bool moveAvailable { get; set; } = true;
    [field: SerializeField] public bool attackAvailable { get; set; } = true;
    //
    protected Color originalColor = Color.magenta; // aka no texture, lol

    protected virtual void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapPathfinder = GetComponent<UnitPathfinder>();
        unitStats = GetComponent<UnitStats>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponent<UnitMap>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();

        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();

        // some init things that need to be taken care of
        unitStats.UpdateHP(unitStats.VITALITY);
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


    // IUnitPhaseInfo
    public void RefreshInfo() {
        turnActive = true;
        moveAvailable = true;
        attackAvailable = true;
        spriteRenderer.color = originalColor;
    }

    // IUnitPhaseInfo
    public virtual void FinishTurn() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
    }

    public Attack GenerateAttack() {
        return new Attack(
            unitStats.STRENGTH,         // damage
            unitStats.DEXTERITY * 10,   // hit rate
            0                           // crit rate
        );
    }

    public bool ReceiveAttack(Attack incomingAttack) {
		// calc hit/crit
		int diceRoll = Random.Range(0, 100);
		bool isHit = diceRoll < incomingAttack.hitRate;

		// final retval
		bool survived = true;
		if (isHit) {
			bool isCrit = diceRoll < incomingAttack.critRate;
			float finalDamage = (float)incomingAttack.damage;

			if (isCrit) {
				finalDamage *= 3f;
				Debug.Log($"Critical hit! ({incomingAttack.critRate}%) for {finalDamage} damage");
			}

            // ouchies, play the animations for hurt
            TriggerHurtAnimation(isCritical: isCrit);
			survived = SufferDamage((int)finalDamage);

        // miss
		} else {
            TriggerMissAnimation();
			Debug.Log($"{this} dodged the attack! ({incomingAttack.hitRate}% to hit)");
		}

		return survived;
	}

	protected bool SufferDamage(int incomingDamage) {
        unitStats.UpdateHP(unitStats._CURRENT_HP - incomingDamage);
		return unitStats._CURRENT_HP > 0;
	}

    public void TriggerAttackAnimation(GridPosition towards) {
        StartCoroutine(
            spriteAnimator.BumpTowards<GridPosition>(towards, battleMap, distanceScale: 7.0f)
        );
    }

    protected void TriggerHurtAnimation(bool isCritical = false) {
		StartCoroutine( spriteAnimator.FlashColor(Constants.threatColorRed) );
		StartCoroutine( spriteAnimator.Shake((isCritical) ? 0.15f : 0.075f) );
    }

    protected void TriggerMissAnimation() {
		StartCoroutine( spriteAnimator.FlashColor(Constants.selectColorWhite) );
    }

	public void TriggerDeathAnimation() {
		StartCoroutine( spriteAnimator.ExecuteAfterAnimating(() => {
			StartCoroutine( spriteAnimator.FadeDown(1.0f) );
		}));
	}

	public void DeathCleanUp() {
        Destroy(gameObject);
        unitMap.ClearPosition(gridPosition);
	}
}
