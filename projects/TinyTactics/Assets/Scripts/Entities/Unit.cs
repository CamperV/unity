using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(UnitPathfinder))]
[RequireComponent(typeof(StatSystem))]
[RequireComponent(typeof(MessageEmitter))]
[RequireComponent(typeof(MutationSystem))]
[RequireComponent(typeof(StatusSystem))]
[RequireComponent(typeof(SpriteOutline))]
[RequireComponent(typeof(Inventory))]
public abstract class Unit : MonoBehaviour, IGridPosition, IUnitPhaseInfo, ITagged, IGUID
{
    [SerializeField] public string displayName;
    [HideInInspector] public Sprite mainSprite;
    [SerializeField] public Sprite portraitSprite;

    [field: SerializeField] public GridPosition gridPosition { get; set; }
    protected GridPosition _reservedGridPosition; // this is for maintaining state while animating/moving
    protected GridPosition _startingGridPosition; // this is for maintaining a revertable state when prevewing Engagements, etc

    // these are used so that our various components can modify the mutable Attack/Defenses that are created during an Engagement
    public delegate void AttackGeneration(Unit thisUnit, ref MutableAttack mutAtt, Unit other);
    public event AttackGeneration OnAttackGeneration;
    public event AttackGeneration OnDefenseGeneration;

    // simply used to signal when a unit has been hit, dodged. etc
    public delegate void OnAction();
    public event OnAction OnAvoid;
    public event OnAction OnWait;
    public event OnAction OnMiss;

    public delegate void OnTargetedAction(Unit thisUnit, Unit other);
    public event OnTargetedAction OnHitTarget;
    public event OnTargetedAction OnCriticalTarget;
    public event OnTargetedAction OnHurtByTarget;
    public event OnTargetedAction OnDefeatTarget;

    public delegate void Movement(Unit thisUnit, Path<GridPosition> path);
    public event Movement OnMove;

    public delegate void OnPhaseInfo(Unit thisUnit);
    public event OnPhaseInfo OnStartTurn;
    public event OnPhaseInfo OnFinishTurn;
    public event OnPhaseInfo OnDeath;
    //

    // necessary Component references
    [HideInInspector] public UnitMap unitMap;
    [HideInInspector] public BattleMap battleMap;
    [HideInInspector] public SpriteAnimator spriteAnimator;
    [HideInInspector] public UnitPathfinder unitPathfinder;
    [HideInInspector] public PersonalAudioFX personalAudioFX;
    [HideInInspector] public MessageEmitter messageEmitter;
    [HideInInspector] public StatSystem statSystem;
    [HideInInspector] public MutationSystem mutationSystem;
    [HideInInspector] public StatusSystem statusSystem;
    [HideInInspector] public SpriteOutline spriteOutline;
    [HideInInspector] public Inventory inventory;
    
    // I don't love this, but it makes things much cleaner.
    [HideInInspector] public PlayerUnitController playerUnitController;
    [HideInInspector] public EnemyUnitController enemyUnitController;

    // other
    [HideInInspector] public MoveRange moveRange;
    [HideInInspector] public TargetRange attackRange;

    // Equipment
    public Weapon EquippedWeapon => inventory.FirstWeapon;

    // for effectiveness, such as "Flier"
    [field: SerializeField] public List<string> tags { get; set; }

    // for knowing which bag of perks to grab from
    public ArchetypeData[] archetypes;
    public MutationArchetype[] mutArchetypes;

    // debug
    public DebugStateLabel debugStateLabel;

    // abstract
    public abstract void RevertTurn();

    // IUnitPhaseInfo
    [field: SerializeField] public bool turnActive { get; set; } = false;
    [field: SerializeField] public bool moveAvailable { get; set; } = false;
    
    // IGUID
    public Guid GUID { get; set; }

    // convenience
    public bool MouseHovering => battleMap.CurrentMouseGridPosition == gridPosition;
    public bool Alive => statSystem.CURRENT_HP > 0;

    protected virtual void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        unitPathfinder = GetComponent<UnitPathfinder>();
        personalAudioFX = GetComponent<PersonalAudioFX>();
        messageEmitter = GetComponent<MessageEmitter>();
        statSystem = GetComponent<StatSystem>();
        mutationSystem = GetComponent<MutationSystem>();
        statusSystem = GetComponent<StatusSystem>();
        spriteOutline = GetComponent<SpriteOutline>();
        inventory = GetComponent<Inventory>();

        mainSprite = GetComponentInChildren<SpriteRenderer>().sprite;
        if (portraitSprite == null) {
            portraitSprite = mainSprite;
        }
        

        // debug
        debugStateLabel = GetComponent<DebugStateLabel>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponent<UnitMap>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();

        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();

        moveRange = null;
        attackRange = null;

        // IGUID
        GUID = Guid.NewGuid();
    }

    protected virtual void Start() {
        // call this Init here, instead of MS's own Start(), to avoid races
        statSystem.Initialize();
        mutationSystem.Initialize();
        statusSystem.Initialize();
        inventory.Initialize();
    }

    // we must take care to add certain functions to the MoveRange
    // The MoveRange field.Keys indicate what tiles can be pathed through
    // However, MoveRange doesn't know what tiles it cannot stand on
    // we pass it a UnitAt lambda to tell it you can't validly stand on occupied tiles
    public void UpdateThreatRange(bool standing = false, int minRange = -1, int maxRange = -1) {
        int movement = (moveAvailable && standing == false) ? statSystem.MOVE : 0;
        moveRange = unitPathfinder.GenerateFlowField<MoveRange>(gridPosition, range: movement);
        moveRange.RegisterValidMoveToFunc(unitMap.CanMoveInto);

        attackRange = new TargetRange(
            moveRange, 
            (minRange < 0) ? EquippedWeapon.MIN_RANGE : minRange,
            (maxRange < 0) ? EquippedWeapon.MAX_RANGE : maxRange
        );
    }

    public bool HasTagMatch(params string[] tagsToCheck) {
        foreach (string tag in tagsToCheck) {
            if (tags.Contains(tag))
                return true;
        }
        return false;
    }

    public IEnumerable<Unit> AlliesWithinRange(int range) {
        foreach (GridPosition gp in gridPosition.Radiate(range)) {
            if (gp == gridPosition || !battleMap.IsInBounds(gp)) continue;
            Unit unit = unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() == GetType()) {
                yield return unit;
            }
        }
    }

    public IEnumerable<Unit> EnemiesWithinRange(int range) {
        foreach (GridPosition gp in gridPosition.Radiate(range)) {
            if (gp == gridPosition || !battleMap.IsInBounds(gp)) continue;
            Unit unit = unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() != GetType()) {
                yield return unit;
            }
        }
    }

    public IEnumerable<Unit> Allies() {
        foreach (GridPosition gp in battleMap.Positions) {
            if (gp == gridPosition) continue;
            Unit unit = unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() == GetType()) {
                yield return unit;
            }
        }
    }

    public IEnumerable<Unit> Enemies() {
        foreach (GridPosition gp in battleMap.Positions) {
            if (gp == gridPosition) continue;
            Unit unit = unitMap.UnitAt(gp);

            if (unit != null && unit.GetType() != GetType()) {
                yield return unit;
            }
        }
    }

    public IEnumerable<Unit> EnemiesThreateningCombo() {
        foreach (Unit unit in unitMap.Units) {
            if (unit == this || unit.GetType() == GetType()) continue;

            TargetRange standing = TargetRange.Standing(unit.gridPosition, unit.EquippedWeapon.MIN_RANGE, unit.EquippedWeapon.MAX_RANGE);
            if (standing.ValidTarget(gridPosition)) yield return unit;
        }
    }

    public IEnumerable<TerrainTile> TerrainWithinRange(int range) {
        foreach (GridPosition gp in gridPosition.Radiate(range)) {
            if (gp == gridPosition || !battleMap.IsInBounds(gp)) continue;
            yield return battleMap.TerrainAt(gp);
        }
    }

    public IEnumerable<Attack> GenerateAttacks(Unit target, Attack.AttackType aType, Attack.AttackDirection aDirection) {
        for (int s = 0; s < (statSystem.MULTISTRIKE+1); s++) {
            yield return Attack.GenerateAttack(this, target, aType, aDirection);
        }
    }

    // IUnitPhaseInfo
    public virtual void RefreshInfo() {
        moveAvailable = true;
        spriteAnimator.RevertColor();  // to original
        UpdateThreatRange();
    }

    public virtual void StartTurn() {
        turnActive = true;
        
        // regain poise
        statSystem.UpdatePoise(statSystem.MAX_POISE, statSystem.MAX_POISE);

        // finally, store your starting location
        // this is relevant for RevertTurn calls
        _startingGridPosition = gridPosition;
        FireOnStartTurnEvent();
    }

    // IUnitPhaseInfo
    public virtual void FinishTurn() {
        turnActive = false;
        moveAvailable = false;
        spriteAnimator.SetColor(SpriteAnimator.InactiveColor);

        FireOnFinishTurnEvent();
    }

    public void SufferPoiseDamage(int incomingDamage, GameObject fromSource) {
        // only animate if you're alive and you're going from Some -> None, not if you're already empty of Poise
        if (statSystem.CounterAttackAvailable) {
            statSystem.UpdatePoise(statSystem.CURRENT_POISE - incomingDamage, statSystem.MAX_POISE);
            if (Alive && !statSystem.CounterAttackAvailable) TriggerPoiseBreak();
        }
    }

    public void TriggerPoiseBreak() {
        StartCoroutine( PoiseBreak() );
    }

    private IEnumerator PoiseBreak() {
        personalAudioFX.PlayBreakFX();
        messageEmitter.Emit(MessageEmitter.MessageType.Debuff, $"BREAK");

        StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.threatColorIndigo) );
        StartCoroutine( spriteAnimator.SmoothCosX(32f, 0.015f, 0f, 1.0f) );
        yield break;
    }

	public bool SufferDamage(int incomingDamage, GameObject fromSource, bool isCritical = false) {
        // animation + message control
        if (isCritical) {
            messageEmitter.EmitTowards(MessageEmitter.MessageType.CritDamage, $"{incomingDamage}!", fromSource.transform.position);
            TriggerVeryHurtAnimation();

        } else {
            // normal damage
            if (incomingDamage > 0) {
                messageEmitter.EmitTowards(MessageEmitter.MessageType.Damage, $"{incomingDamage}", fromSource.transform.position);
                TriggerHurtAnimation();

            // no damage
            } else {
                messageEmitter.EmitTowards(MessageEmitter.MessageType.NoDamage, $"{incomingDamage}", fromSource.transform.position);
                TriggerNoDamageHurtAnimation();
            }
        }

        // perform actual subtraction
        statSystem.UpdateHP(statSystem.CURRENT_HP - incomingDamage, statSystem.MAX_HP);

        // lethal
        if (!Alive) {
            TriggerDeath();
            personalAudioFX.PlayLethalDamageFX();   // this is different than DeathFX
        }
        return Alive;
	}

    public void TriggerDeath() {
        StartCoroutine( SequentialDeath() );
    }

    private IEnumerator SequentialDeath() {
        yield return new WaitUntil(spriteAnimator.DoneAnimatingAndEmptyQueue);

        // first, fire the Death event
        FireOnDeathEvent();

        // wait until you're ready to animate
        personalAudioFX.PlayDeathFX();

        StartCoroutine( spriteAnimator.SmoothCosX(12f, 0.06f, 0f, 1.0f) );
        yield return spriteAnimator.FadeDownAll(1.0f);
        // yield return new WaitWhile(personalAudioFX.IsPlaying);

        // after animating:
        gameObject.SetActive(false);

        // playerUnitController checks endPhase here
        FinishTurn();

        // Battle checks victory conditions here
        unitMap.ClearPosition(gridPosition);
    }

    public void HealAmount(int healAmount) {
        if (statSystem.CURRENT_HP < statSystem.MAX_HP) {
            messageEmitter.Emit(MessageEmitter.MessageType.Heal, $"+{healAmount}");

            // queue the sound and animation for after it is done animating the Hurt animation
            spriteAnimator.QueueAction(
                () => {   
                    TriggerHealAnimation();
                    personalAudioFX.PlayHealFX();
                }
            );

            statSystem.UpdateHP(statSystem.CURRENT_HP + healAmount, statSystem.MAX_HP);
        }
    }

    public void TriggerAttackAnimation(GridPosition towards) {
        StartCoroutine(
            spriteAnimator.BumpTowards<GridPosition>(towards, battleMap, distanceScale: 7.0f)
        );
    }

    public void TriggerHurtAnimation() {
		StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.threatColorRed) );
        StartCoroutine( spriteAnimator.Shake(0.075f, 3) );
    }

    // this is used for when no damage is taken, but a unit is hit
    public void TriggerNoDamageHurtAnimation() {
		StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.selectColorWhite) );
        StartCoroutine( spriteAnimator.Shake(0.05f, 3) );
    }

    // this is used for Crits
    public void TriggerVeryHurtAnimation() {
		StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.threatColorViolet) );
        StartCoroutine( spriteAnimator.Shake(0.20f, 5) );
    }

    public void TriggerMissAnimation() {
		StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.selectColorWhite) );
        StartCoroutine( spriteAnimator.SmoothBumpRandom(0.10f) );
    }

    public void TriggerHealAnimation() {
		StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.healColorGreen) );
    }

    public void TriggerDebuffAnimation(AudioClip playClip, params string[] affectedStats) {
        personalAudioFX.PlayFX(playClip);

        foreach (string affectedStat in affectedStats) {
            if (affectedStat != "") {
                messageEmitter.Emit(MessageEmitter.MessageType.Debuff, $"-{affectedStat}");
            }
        }

        StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.threatColorIndigo) );
        StartCoroutine( spriteAnimator.SmoothCosX(18f, 0.03f, 0f, 1.0f) );
    }

    public void TriggerBuffAnimation(AudioClip playClip, params string[] affectedStats) {
        personalAudioFX.PlayFX(playClip);

        foreach (string affectedStat in affectedStats) {
            messageEmitter.Emit(MessageEmitter.MessageType.Buff, $"+{affectedStat}");
        }

        StartCoroutine( spriteAnimator.FlashColorThenRevert(Palette.threatColorYellow) );
        StartCoroutine( spriteAnimator.SmoothCosX(32f, 0.015f, 0f, 1.0f) );
    }

    public void FireOnAttackGenerationEvent(ref MutableAttack mutAtt, Unit other) => OnAttackGeneration?.Invoke(this, ref mutAtt, other);
    public void FireOnDefenseGenerationEvent(ref MutableAttack mutAtt, Unit other) => OnDefenseGeneration?.Invoke(this, ref mutAtt, other);

    public void FireOnAvoidEvent() {
        TriggerMissAnimation();
        OnAvoid?.Invoke();
    }
    public void FireOnWaitEvent() => OnWait?.Invoke();
    public void FireOnMissEvent() => OnMiss?.Invoke();
    
    // targeted versions
    public void FireOnHitTargetEvent(Unit target) => OnHitTarget?.Invoke(this, target);
    public void FireOnCriticalTargetEvent(Unit target) => OnCriticalTarget?.Invoke(this, target);
    public void FireOnHurtByTargetEvent(Unit other) => OnHurtByTarget?.Invoke(this, other);
    public void FireOnDefeatTargetEvent(Unit target) => OnDefeatTarget?.Invoke(this, target);

    public void FireOnMoveEvent(Path<GridPosition> pathTaken) => OnMove?.Invoke(this, pathTaken);

    public void FireOnStartTurnEvent() => OnStartTurn?.Invoke(this);
    public void FireOnFinishTurnEvent() => OnFinishTurn?.Invoke(this);

    public void FireOnDeathEvent() => OnDeath?.Invoke(this);
}
