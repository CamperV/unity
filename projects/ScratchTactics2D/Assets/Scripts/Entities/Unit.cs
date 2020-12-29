using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	private bool animFlag = false;

	public Guid id; // to be set by UnitStats

	// NOTE this is set by a Controller during registration
	public Controller parentController;

	public UnitUI unitUIPrefab;
	[HideInInspector] public UnitUI unitUI;

	public HashSet<Vector3Int> obstacles {
		get {
			return parentController.GetObstacles();
		}
	}

	// to be defined at a lower level
	public abstract int movementRange 	{ get; set; }
	public abstract int attackReach 	{ get; set; }
	public abstract int damageValue 	{ get; set; }
	public abstract int maximumHealth 	{ get; set; }

	private int _currentHealth;
	protected int currentHealth {
		get => _currentHealth;
		set {
			_currentHealth = value;
			unitUI.healthBar.UpdateBar(_currentHealth);
		}
	}

	// cache the movement range for easier lookup later
	public MoveRange moveRange;
	public AttackRange attackRange;

	protected Dictionary<string, bool> optionAvailability = new Dictionary<string, bool>() {
		["Move"]	= true,
		["Attack"]	= true
	};

	void Awake() {
		base.Awake();

		unitUI = Instantiate(unitUIPrefab, this.transform);
		unitUI.transform.parent = this.transform;
		unitUI.healthBar.InitHealthBar(maximumHealth);
	}

	void Start() {
		currentHealth = maximumHealth;
	}
	
	public override bool IsActive() {
		return gameObject.activeInHierarchy && currentHealth > 0;
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

	public bool AnyOptionActive() {
		foreach (var k in optionAvailability.Keys.ToList()) {
			if (optionAvailability[k]) return true;
		}
		return false;
	}

	public void ApplyStats(UnitStats stats) {
		id = stats.id;
		movementRange += stats.moveMod;

		unitUI.healthBar.InitHealthBar(maximumHealth);
	}

	// valid Unit Actions:
	// OnSelect
	// ShowMovementRange
	// TraverseTo
	// Attack
	// Wait
	// Other
	public void RefreshOptions() {
		foreach (var k in optionAvailability.Keys.ToList()) {
			optionAvailability[k] = true;
		}
		spriteRenderer.color = Color.white;
	}

	public void OnEndTurn() {
		StartCoroutine(ExecuteAfterMoving(() => {
			foreach (var k in optionAvailability.Keys.ToList()) {
				optionAvailability[k] = false;
			}
			spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		})); 
	}

	public void UpdateThreatRange() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(grid.GetAllTilePos());
		tiles.ExceptWith(obstacles);

		var moveable = (OptionActive("Move")) ? movementRange : 0;
		var attackable = (OptionActive("Attack")) ? attackReach : 0;
		moveRange = MoveRange.MoveRangeFrom(gridPosition, tiles, range: moveable);
		attackRange = AttackRange.AttackRangeFrom(moveRange, grid.GetAllTilePos(), range: attackable);
	}

	public void OnSelect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		// play "awake" ready animation
		// enter into "running" or "ready" animation loop
		// display movement range
		// spriteRenderer.color = Utils.selectColorRed;
		
		UpdateThreatRange();
		attackRange?.Display(grid);
		moveRange?.Display(grid);
	}

	public void OnDeselect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		//
		spriteRenderer.color = Color.white;
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);
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

	public void Attack(Unit other) {
		other.SufferDamage(damageValue);
	}

	public void SufferDamage(int incomingDamage) {
		currentHealth -= incomingDamage;
		StartCoroutine(FlashColor(Utils.threatColorRed));
		StartCoroutine(Shake(0.075f));

		Debug.Log($"{this} suffered {incomingDamage}, {currentHealth}/{maximumHealth} health remaining.");

		if (currentHealth < 1) { Die(); }
	}

	public void Die() {
		// fade down
		// when faded, remove gameObject
		Debug.Log($"{this} has died :(");
		GameManager.inst.tacticsManager.GetActiveGrid().UpdateOccupantAt(gridPosition, null);

		// this unit will be automatically removed from the activeRegistry of the controller...
		// but won't be removed from the actual OverworldEntity unless we force it
		var battle = GameManager.inst.tacticsManager.activeBattle;
		OverworldEntity oe = battle.GetOverworldEntityFromController(parentController);
		oe.RemoveUnit(this.id);

		StartCoroutine(ExecuteAfterAnimating(() => {
			StartCoroutine(FadeDownToInactive(0.01f));
		}));
	}

	public IEnumerator FlashColor(Color color) {
		animFlag = true;

		float c = 1.0f;
		float rate = 0.005f;
		while (c > 0.0f) {
			var colorDiff = Color.white - (c * (Color.white - color));
			spriteRenderer.color = new Color(colorDiff.r, colorDiff.g, colorDiff.b, 1);
			c -= rate;
			rate *= 1.05f;
			yield return null;
		}
		spriteRenderer.color = Color.white;
		animFlag = false;
	}

	public IEnumerator Shake(float radius) {
		var ogPosition = transform.position;
		for (int i=0; i<3; i++) {
			transform.position = transform.position + (Vector3)Random.insideUnitCircle*radius;
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		transform.position = ogPosition;
	}

	public IEnumerator ExecuteAfterAnimating(Action voidAction) {
		while (animFlag) {
			yield return null;
		}
		voidAction();
	}
}
