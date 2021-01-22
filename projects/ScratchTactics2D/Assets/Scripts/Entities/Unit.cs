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
	public bool selectionLock { get; private set; }
	[HideInInspector] public bool inFocus {
		get => GameManager.inst.tacticsManager.focusSingleton == this;
	}

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
	public HashSet<Vector3Int> obstacles {
		get {
			return parentController.GetObstacles();
		}
	}

	//
	public Enum.PlayerUnitState actionState;

	// Equipment management
	public Weapon equippedWeapon { get => unitStats.inventory.equippedWeapon; }

	// Attribute Area
	// defaultStats is static, and will be defined by the final class
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
			unitUI.UpdateHealthBar(unitStats._HP);
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
	protected Dictionary<string, bool> optionAvailability;

	protected override void Awake() {
		base.Awake();

		unitUI = Instantiate(unitUIPrefab, transform);
		unitUI.healthBar.InitHealthBar(VITALITY);
		unitUI.SetTransparency(0.0f);
		//
		unitUI.BindUnit(this);

		// init keys
		optionAvailability = new Dictionary<string, bool>() {
			["Move"]   = false,
			["Attack"] = false
		};
	}

	void Start() {
		// scale down to avoid weird parent/child problems w/ UnitUI
		// apply inverse scale to all children of our transform
		// Unity Enumerable<Xform> is weird and I wish they'd just use a method for getting children
		transform.localScale *= spriteScaleFactor;
		foreach (Transform childT in transform) {
			childT.localScale /= spriteScaleFactor;
		}
	}

	public void SetFocus(bool takeFocus) {
		// only one unit can hold focus
		// force others to drop focus if their Y value is larger (unit is behind)
		if (takeFocus) {
			unitUI.SetTransparency(1.0f);

			var overlayTile = ScriptableObject.CreateInstance<SelectOverlayIsoTile>() as SelectOverlayIsoTile;
			var overlayPosition = new Vector3Int(gridPosition.x, gridPosition.y, 1);
			GameManager.inst.GetActiveGrid().baseTilemap.SetTile(overlayPosition, overlayTile);
		} else {
			unitUI.SetTransparency(0.0f);

			var overlayPosition = new Vector3Int(gridPosition.x, gridPosition.y, 1);
			GameManager.inst.GetActiveGrid().baseTilemap.SetTile(overlayPosition, null);
		}
	}
	
	public override bool IsActive() {
		return gameObject.activeInHierarchy && _HP > 0;
	}

	public bool OptionActive(string optionToCheck) {
		if (!optionAvailability.ContainsKey(optionToCheck)) {
			throw new System.Exception($"{optionToCheck} not a valid option to check"); 
		}
		return optionAvailability[optionToCheck];
	}

	public void SetOption(string option, bool setting) {
		if (!optionAvailability.ContainsKey(option)) {
			throw new System.Exception($"{option} not a valid option to set"); 
		}
		optionAvailability[option] = setting;
	}

	public void ApplyStats(UnitStats stats) {
		unitStats = stats;
		unitUI.UpdateHealthBar(_HP);
	}

	// Action zone
	public void OnStartTurn() {	
		optionAvailability.Keys.ToList().ForEach(k => optionAvailability[k] = true);
		spriteRenderer.color = Color.white;
		//
		actionState = Enum.PlayerUnitState.idle;
		turnActive = true;
	}

	public void OnEndTurn() {
		optionAvailability.Keys.ToList().ForEach(k => optionAvailability[k] = false);
		spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, spriteRenderer.color.a);
		//
		actionState = Enum.PlayerUnitState.idle;
		turnActive = false;
	}

	public void UpdateThreatRange() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(grid.GetAllTilePos());
		tiles.ExceptWith(obstacles);

		var moveable = (OptionActive("Move")) ? MOVE : 0;
		var attackable = (OptionActive("Attack")) ? _RANGE : 0;
		moveRange = MoveRange.MoveRangeFrom(gridPosition, tiles, range: moveable);
		attackRange = AttackRange.AttackRangeFrom(moveRange, grid.GetAllTilePos(), range: attackable);
	}

	public void OnSelect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// display movement range
		
		UpdateThreatRange();
		attackRange?.Display(grid);
		moveRange?.Display(grid);

		// 
		SetFocus(true);
		selectionLock = true;

		// enter the first default state for move selection
		if (OptionActive("Move")) {
			actionState = Enum.PlayerUnitState.moveSelection;
		} else if (OptionActive("Attack")) {
			actionState = Enum.PlayerUnitState.attackSelection;
			unitUI.DisplayActionOptions(optionAvailability);
		}	
	}

	public void OnDeselect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		//
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);

		//
		SetFocus(false);
		selectionLock = false;

		actionState = Enum.PlayerUnitState.idle;
		unitUI.HideActionOptions();
	}

	public void TraverseTo(Vector3Int target, MovingObjectPath fieldPath = null) {
		GameGrid grid = GameManager.inst.tacticsManager.GetActiveGrid();
		if (fieldPath == null) {
			fieldPath = MovingObjectPath.GetPathFromField(target, moveRange);
		}

		// movement animation
		StartCoroutine(SmoothMovementPath(fieldPath, grid));

		grid.UpdateOccupantAt(gridPosition, null);
		grid.UpdateOccupantAt(target, this);
		gridPosition = target;
	}

	public bool InStandingAttackRange(Vector3Int target) {
		return gridPosition.Radiate(_RANGE).ToList().Contains(target);
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

		StartCoroutine(FlashColor(Utils.threatColorRed));
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
