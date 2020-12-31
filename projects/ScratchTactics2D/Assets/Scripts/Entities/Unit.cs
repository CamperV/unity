using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	private readonly float scaleFactor = 0.75f;
	private bool animFlag = false;

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
	// defaultStats is static, and will be defined by the final class
	public abstract UnitStats unitStats { get; set; }
	public Guid ID { get => unitStats.ID; }
    public int STRENGTH {
		get => unitStats.STRENGTH;
		set => unitStats.STRENGTH = value;
	}
    public int HEALTH {
		get => unitStats.HEALTH;
		set {
			unitStats.HEALTH = value;
			unitUI.UpdateHealthBar(unitStats.HEALTH);
		}
	}
	public int MAXHEALTH {
		get => unitStats.MAXHEALTH;
		set {
			unitStats.MAXHEALTH = value;
			unitUI.healthBar.maxPips = value;
		}
	}
    public int MOVE {
		get => unitStats.MOVE;
		set => unitStats.MOVE = value;
	}
    public int RANGE {
		get => unitStats.RANGE;
		set => unitStats.RANGE = value;
	}

	// cache the movement range for easier lookup later
	public MoveRange moveRange;
	public AttackRange attackRange;

	protected Dictionary<string, bool> optionAvailability = new Dictionary<string, bool>() {
		["Move"]   = true,
		["Attack"] = true
	};

	void Awake() {
		base.Awake();

		unitUI = Instantiate(unitUIPrefab, this.transform);
		unitUI.healthBar.InitHealthBar(MAXHEALTH);
		unitUI.SetTransparency(0.0f);
		unitUI.transform.parent = this.transform;
	}

	void Start() {
		// scale down to avoid weird parent/child problems w/ UnitUI
		// apply inverse scale to all children of our transform
		// Unity Enumerable<Xform> is weird and I wish they'd just use a method for getting children
		transform.localScale *= scaleFactor;
		foreach (Transform childT in transform) {
			childT.localScale /= scaleFactor;
		}
	}
	
	void OnMouseEnter() {
		Debug.DrawLine(boxCollider2D.bounds.min, boxCollider2D.bounds.max);
		Debug.DrawLine(new Vector3(boxCollider2D.bounds.max.x,
								   boxCollider2D.bounds.min.y,
								   boxCollider2D.bounds.min.z),
					   new Vector3(boxCollider2D.bounds.min.x,
					   			   boxCollider2D.bounds.max.y,
								   boxCollider2D.bounds.max.z));

		unitUI.SetTransparency(1.0f);

		// find if you're behind things
		// if you are, fade those in front things
		foreach (Unit activeUnit in parentController.GetRegisteredInBattle()) {

			// if a unit is potentially in front of us
			if (!activeUnit.Equals(this) && activeUnit.transform.position.y <= transform.position.y) {

				// and also is intersecting w/ you
				// don't use compound if for line-length readability
				if (boxCollider2D.bounds.Intersects(activeUnit.boxCollider2D.bounds)) {
					activeUnit.SetTransparency(0.5f);
				}
			}
		}
	}
	void OnMouseExit() {
		foreach (Unit activeUnit in parentController.GetRegisteredInBattle()) {
			activeUnit.SetTransparency(1.0f);
			activeUnit.unitUI.SetTransparency(0.0f);
		}		
	}
	
	public override bool IsActive() {
		return gameObject.activeInHierarchy && HEALTH > 0;
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
		unitStats = stats;
		unitUI.UpdateHealthBar(HEALTH);
	}

	// Action zone
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

		var moveable = (OptionActive("Move")) ? MOVE : 0;
		var attackable = (OptionActive("Attack")) ? RANGE : 0;
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
		other.SufferDamage(STRENGTH);
	}

	public void SufferDamage(int incomingDamage) {
		HEALTH -= incomingDamage;
		StartCoroutine(FlashColor(Utils.threatColorRed));
		StartCoroutine(Shake(0.075f));

		Debug.Log($"{this} suffered {incomingDamage}, {HEALTH}/{MAXHEALTH} health remaining.");

		if (HEALTH < 1) { Die(); }
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
		oe.RemoveUnit(ID);

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
	
	public void SetTransparency(float val) {
		var currColor = spriteRenderer.color;
		currColor.a = val;
		spriteRenderer.color = currColor;
	}
}
