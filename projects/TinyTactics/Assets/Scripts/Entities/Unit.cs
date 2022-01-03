using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class Unit : MonoBehaviour, IGridPosition, IUnitPhaseInfo
{
    [SerializeField] public string displayName;

    [field: SerializeField] public GridPosition gridPosition { get; set; }
    protected GridPosition _reservedGridPosition; // this is for maintaining state while animating/moving
    protected GridPosition _startingGridPosition; // this is for maintaining a revertable state when prevewing Engagements, etc

    // these are used so that our various Perks can modify the mutable Attack/Defenses that are created during an Engagement
    public delegate void AttackGeneration(ref MutableAttack mutAtt, Unit target);
    public event AttackGeneration OnAttack;

    public delegate void DefenseGeneration(ref MutableDefense mutDef, Unit attacker);
    public event DefenseGeneration OnDefend;

    // simply used to signal when a unit has been hit, dodged. etc
    public delegate void OnAction();
    public event OnAction OnHurt;
    public event OnAction OnAvoid;

    public delegate void OnTargetedAction(Unit target);
    public event OnTargetedAction OnHit;

    public delegate void Movement(Path<GridPosition> path);
    public event Movement OnMove;

    public delegate void OnPhaseInfo(Unit thisUnit);
    public event OnPhaseInfo OnStartTurn;
    public event OnPhaseInfo OnFinishTurn;
    //

    // necessary Component references
    [HideInInspector] protected UnitMap unitMap;
    [HideInInspector] protected BattleMap battleMap;
    [HideInInspector] public SpriteAnimator spriteAnimator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public UnitPathfinder unitPathfinder;
    [HideInInspector] public UnitStats unitStats;
    [HideInInspector] protected HoldTimer holdTimer;
    [HideInInspector] public BuffManager buffManager;
    
    // I don't love this, but it makes things much cleaner.
    protected PlayerUnitController playerUnitController;
    protected EnemyUnitController enemyUnitController;

    // other
    public MoveRange moveRange;
    public AttackRange attackRange;

    // Equipment
    public Weapon equippedWeapon;

    // for effectiveness, such as "Flier"
    public List<string> tags;

    // debug
    public DebugStateLabel debugStateLabel;

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

    public bool MouseHovering => battleMap.CurrentMouseGridPosition == gridPosition;
    public string logTag => (GetType() == typeof(PlayerUnit)) ? "PLAYER_UNIT" : "ENEMY_UNIT";

    protected virtual void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        unitPathfinder = GetComponent<UnitPathfinder>();
        unitStats = GetComponent<UnitStats>();
        holdTimer = GetComponent<HoldTimer>();
        buffManager = GetComponent<BuffManager>();

        // debug
        debugStateLabel = GetComponent<DebugStateLabel>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponent<UnitMap>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();

        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();

        equippedWeapon = Instantiate(equippedWeapon, transform);
        equippedWeapon.Equip(this);
    }

    protected virtual void Start() {          
        // acquire all perks
        foreach (Perk perk in GetComponents<Perk>()) {
            perk.OnAcquire();
        }

        // some init things that need to be taken care of
        unitStats.UpdateHP(unitStats.VITALITY, unitStats.VITALITY);

        originalColor = spriteRenderer.color;
        moveRange = null;
        attackRange = null;
    }

    // we must take care to add certain functions to the MoveRange
    // The MoveRange field.Keys indicate what tiles can be pathed through
    // However, MoveRange doesn't know what tiles it cannot stand on
    // we pass it a UnitAt lambda to tell it you can't validly stand on occupied tiles
    public void UpdateThreatRange(bool standing = false) {
        int movement = (moveAvailable && standing == false) ? unitStats.MOVE : 0;
        moveRange = unitPathfinder.GenerateFlowField<MoveRange>(gridPosition, range: movement);
        moveRange.RegisterValidMoveToFunc(unitMap.CanMoveInto);

        if (attackAvailable) {
            attackRange = new AttackRange(moveRange, equippedWeapon.weaponStats.MIN_RANGE, equippedWeapon.weaponStats.MAX_RANGE);
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

    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }

    // IUnitPhaseInfo
    public void RefreshInfo() {
        turnActive = true;
        moveAvailable = true;
        attackAvailable = true;
        RevertColor();  // to original
        UpdateThreatRange();
    }

    public void StartTurn() {
        // finally, store your starting location
        // this is relevant for RevertTurn calls
        _startingGridPosition = gridPosition;
        FireOnStartTurnEvent();
    }

    // IUnitPhaseInfo
    public virtual void FinishTurn() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);

        FireOnFinishTurnEvent();
    }

	public bool SufferDamage(int incomingDamage, bool isCritical = false) {
        TriggerHurtAnimation(isCritical: isCritical);

        unitStats.UpdateHP(unitStats._CURRENT_HP - incomingDamage, unitStats.VITALITY);
		bool survived = unitStats._CURRENT_HP > 0;

        UIManager.inst.combatLog.AddEntry($"{logTag}@[{displayName}] suffers YELLOW@[{incomingDamage}] damage.");

        if (!survived) {
            TriggerDeathAnimation();
            DeathCleanUp();

        // I've been hurt, but survived
        } else {
            FireOnHurtEvent();    
        }

        return survived;
	}

    public void HealAmount(int healAmount) {
        TriggerHealAnimation();

        unitStats.UpdateHP(unitStats._CURRENT_HP + healAmount, unitStats.VITALITY);
        UIManager.inst.combatLog.AddEntry($"{logTag}@[{displayName}] healed for GREEN@[{healAmount}].");
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

    public void TriggerHealAnimation() {
		StartCoroutine( spriteAnimator.FlashColor(Constants.healColorGreen) );
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
        UIManager.inst.combatLog.AddEntry($"{logTag}@[{displayName}] is KEYWORD@[destroyed].");

    	StartCoroutine( spriteAnimator.ExecuteAfterAnimating(() => {
            gameObject.SetActive(false);
            DisableFSM();
            unitMap.ClearPosition(gridPosition);
            FinishTurn();
		}));
	}

    public void FireOnAttackEvent(ref MutableAttack mutAtt, Unit target) => OnAttack?.Invoke(ref mutAtt, target);
    public void FireOnDefendEvent(ref MutableDefense mutDef, Unit attacker) => OnDefend?.Invoke(ref mutDef, attacker);
    public void FireOnHurtEvent() => OnHurt?.Invoke();
    public void FireOnAvoidEvent() {
        TriggerMissAnimation();
		UIManager.inst.combatLog.AddEntry($"{logTag}@[{displayName}] KEYWORD@[avoided] the attack.");

        OnAvoid?.Invoke();
    }
    
    // targeted versions
    public void FireOnHitEvent(Unit target) => OnHit?.Invoke(target);

    public void FireOnMoveEvent(Path<GridPosition> pathTaken) => OnMove?.Invoke(pathTaken);

    public void FireOnStartTurnEvent() => OnStartTurn?.Invoke(this);
    public void FireOnFinishTurnEvent() => OnFinishTurn?.Invoke(this);
}
