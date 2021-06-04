using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Extensions;

public abstract class Unit : TacticsEntityBase
{
	// flags, constants, etc
	private readonly float spriteScaleFactor = 0.55f;
	public bool selectionLock { get; protected set; }
	[HideInInspector] public bool inFocus {
		get => GameManager.inst.tacticsManager.focusSingleton == this;
	}

	public bool inMildFocus = false;

	// NOTE this is set by a Controller during registration
	public UnitController parentController;
	public bool isPlayerControlled {
		get {
			return parentController.GetType() == typeof(PlayerUnitController);
		}
	}

	public UnitUI unitUIPrefab;
	[HideInInspector] public UnitUI unitUI;

	//
	// what can't I run through?
	public HashSet<Vector3Int> obstacles { get => parentController.GetObstacles(); }

	// Equipment management
	public Weapon equippedWeapon { get => unitStats.inventory.equippedWeapon; }

	// Attribute Area
	// defaultStats is static, and will be defined by the final class
	public abstract Sprite portrait { get; }
	public abstract UnitStats unitStats { get; set; }
	public Guid ID { get => unitStats.ID; }

	public int VITALITY {
		get => unitStats.VITALITY;
		set {
			unitStats.VITALITY = value;
			unitUI.healthBar.maxPips = value;
		}
	}
    public int STRENGTH {
		get => unitStats.STRENGTH;
		set => unitStats.STRENGTH = value;
	}
    public int DEXTERITY {
		get => unitStats.DEXTERITY;
		set => unitStats.DEXTERITY = value;
	}
    public int REFLEX {
		get => unitStats.REFLEX;
		set => unitStats.REFLEX = value;
	}
    public int MOVE {
		get => unitStats.MOVE;
		set => unitStats.MOVE = value;
	}

	// derived stat: calculate from equipped weapon + modifiers
	public int _HP {
		get => unitStats._HP;
		set {
			unitStats._HP = value;
			unitUI.UpdateHealthBarThenFade(unitStats._HP);
		}
	}
    public int _CAPACITY {
		get => unitStats._CAPACITY;
		set => unitStats._CAPACITY = value;
	}
    public int _RANGE {
		get => equippedWeapon?.REACH ?? 1;
	}

	// cache the movement range for easier lookup later
	public MoveRange moveRange;
	public AttackRange attackRange;

	public bool turnActive { get; protected set; }
	public Dictionary<string, bool> optionAvailability;

	protected override void Awake() {
		base.Awake();

		unitUI = Instantiate(unitUIPrefab, transform);
		unitUI.healthBar.InitHealthBar(VITALITY);
		unitUI.SetTransparency(0.0f);
		unitUI.BindUnit(this);

		// init keys
		optionAvailability = new Dictionary<string, bool>() {
			["Move"]   = true,
			["Attack"] = true
		};
	}

	void Start() {
		// scale down to avoid weird parent/child problems w/ UnitUI
		// apply inverse scale to all children of our transform
		// Unity Enumerable<Transform> is weird and I wish they'd just use a method for getting children
        transform.localScale = new Vector3(spriteScaleFactor, spriteScaleFactor, 1.0f);
		foreach (Transform childT in transform) {
			childT.localScale = new Vector3(1.0f/spriteScaleFactor, 1.0f/spriteScaleFactor, 1.0f);
		}
	}

	public void SetFocus(bool takeFocus) {
		// only one unit can hold focus
		// force others to drop focus if their Y value is larger (unit is behind)
		if (takeFocus) {
			DisplayThreatRange();
		} else {
			ClearDisplayThreatRange();
		}
	}

	// self terminating
	public void SetMildFocus(bool takeFocus) {
		if (takeFocus) {
			inMildFocus = true;	// prevents ghosting

			// add the lil selection square
			GameManager.inst.GetActiveGrid().UnderlayAt(gridPosition, Constants.threatColorYellow);
		} else {
			inMildFocus = false;

			// just in case
			GameManager.inst.GetActiveGrid().ResetUnderlayAt(gridPosition);
		}
	}
	
	public override bool IsActive() {
		return gameObject.activeInHierarchy && _HP > 0;
	}

	public bool OptionActive(string optionToCheck) {
		return optionAvailability[optionToCheck];
	}

	public void SetOption(string option, bool setting) {
		optionAvailability[option] = setting;
	}

	public void ApplyStats(UnitStats stats) {
		unitStats = stats;

		unitUI.UpdateWeaponEmblem(equippedWeapon);
		unitUI.UpdateHealthBar(_HP);
		unitUI.SetTransparency(0.0f);
	}

	// Action zone
	public virtual void OnStartTurn() {
		spriteRenderer.color = Color.white;
		//
		turnActive = true;
	}

	public virtual void OnEndTurn() {
		optionAvailability.Keys.ToList().ForEach(k => SetOption(k, true));
		spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, spriteRenderer.color.a);
		//
		turnActive = false;
	}

	public void UpdateThreatRange() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();

		var moveable = (OptionActive("Move")) ? MOVE : 0;
		var attackable = (OptionActive("Attack")) ? _RANGE : 0;

		moveRange = new EntityPathfinder(grid, obstacles).FlowField<MoveRange>(gridPosition, range: moveable);
		Debug.Log($"created mrange {moveRange} / {moveRange == null}");
		attackRange = new AttackRange(moveRange, attackable);
	}

	public void DisplayThreatRange() {
		var grid = GameManager.inst.GetActiveGrid();
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);

		UpdateThreatRange();
		attackRange.Display(grid);
		moveRange.Display(grid);

		foreach (Vector3Int v in GetThreatenedTiles()) {
			if (moveRange.field.ContainsKey(v)) {
				grid.UnderlayAt(v, Constants.threatColorIndigo);
				continue; // don't highlight the move and the attack
			}
		}

		// add the lil selection square
		grid.UnderlayAt(gridPosition, Constants.selectColorWhite);
	}

	public void DisplayStandingThreatRange() {
		var grid = GameManager.inst.GetActiveGrid();
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);

		MoveRange standing = new MoveRange(gridPosition);
		attackRange = new AttackRange(standing, _RANGE);
		attackRange.Display(grid);

		// add the lil selection square
		grid.UnderlayAt(gridPosition, Constants.selectColorWhite);
	}

	public void ClearDisplayThreatRange() {
		if (selectionLock) return;

		var grid = GameManager.inst.GetActiveGrid();
		//moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);

		// just in case
		//grid.ResetUnderlayAt(gridPosition);
	}

	public HashSet<Vector3Int> GetThreatenedTiles() {
		HashSet<Vector3Int> threatened = new HashSet<Vector3Int>();
		foreach(Unit u in parentController.GetOpposing()) {
			threatened.UnionWith(u.attackRange.field.Keys);
		}
		return threatened;
	}

	public virtual void OnSelect() {
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// 
		SetFocus(true);
		selectionLock = true;
		//unitUI.healthBar.Show(true);
	}

	public virtual void OnDeselect() {
		//
		SetFocus(false);
		selectionLock = false;
		//unitUI.healthBar.Hide();
	}

	public void TraverseTo(Vector3Int target, Path viaPath) {
		TacticsGrid grid = GameManager.inst.tacticsManager.GetActiveGrid();

		// movement animation
		StartCoroutine( SmoothMovementPath(viaPath, grid) );

		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(target, this);
		gridPosition = target;
	}

	public bool InStandingAttackRange(Vector3Int target) {
		return gridPosition.GridRadiate(GameManager.inst.GetActiveGrid(), _RANGE).ToList().Contains(target);
	}

	public Attack GenerateAttack(bool isAggressor = true) {
		// range has already been calculated, otherwise we couldn't initiate the attack
		// calc hit rate
		// calc is crit?
		// calc damage
		// package it up w/ weapon/attack type (Slash/Pierce/Blunt)
		float strScalingDamage = (STRENGTH * equippedWeapon.strScalingBonus);
		int baseDamage = (int)(equippedWeapon.MIGHT + strScalingDamage);

		float dexScalingHit = (DEXTERITY * equippedWeapon.dexScalingBonus);
		int hitRate =  (int)(equippedWeapon.ACCURACY + dexScalingHit);
		int critRate = DEXTERITY + equippedWeapon.CRITICAL;

		return new Attack(baseDamage, hitRate, critRate);
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
				finalDamage *= incomingAttack.criticalMultiplier;
				Debug.Log($"Critical hit! ({incomingAttack.critRate}%) for {finalDamage} damage");
			}

			survived = SufferDamage((int)finalDamage, isCritical: isCrit);
		} else {
			Debug.Log($"{this} dodged the attack! ({incomingAttack.hitRate}% to hit)");
			unitUI.DisplayDamageMessage("MISS");
		}

		return survived;
	}

	private bool SufferDamage(int incomingDamage, bool isCritical = false) {
		_HP -= incomingDamage;

		StartCoroutine(FlashColor(Constants.threatColorRed));
		StartCoroutine(Shake((isCritical) ? 0.15f : 0.075f));

		unitUI.DisplayDamageMessage(incomingDamage.ToString(), emphasize: isCritical);
		return _HP > 0;
	}
	
	public void TriggerDeathAnimation() {
		StartCoroutine(ExecuteAfterAnimating(() => {
			StartCoroutine(FadeDown(timeToDie));
		}));
	}

	public void DeathCleanUp() {
		// this will affect the get value of activeRegistry which contains it
		// and therefore end the battle immediately, if last unit
		gameObject.SetActive(false);

		GameManager.inst.tacticsManager.GetActiveGrid().UpdateOccupantAt(gridPosition, null);

		// this unit will be automatically removed from the activeRegistry of the controller...
		// but won't be removed from the actual OverworldEntity unless we force it
		var battle = GameManager.inst.tacticsManager.activeBattle;
		OverworldEntity oe = battle.GetOverworldEntityFromController(parentController);
		oe.RemoveUnit(ID);
	}

	public override IEnumerator FadeDown(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			unitUI.SetTransparency(1.0f - timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}
}
