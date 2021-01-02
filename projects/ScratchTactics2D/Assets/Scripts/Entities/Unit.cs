using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public abstract class Unit : TacticsEntityBase
{
	// flags, constants, etc
	private readonly float scaleFactor = 0.75f;
	private bool animFlag = false;
	private bool selectionLock = false;
	[HideInInspector] public bool inFocus = false;

	// NOTE this is set by a Controller during registration
	public Controller parentController;

	public UnitUI unitUIPrefab;
	[HideInInspector] public UnitUI unitUI;

	//
	// what can't I run through?
	public HashSet<Vector3Int> obstacles {
		get {
			return parentController.GetObstacles();
		}
	}

	// Equipment management
	public Weapon equippedWeapon {
		get => unitStats.inventory.equippedWeapon;
		set => unitStats.inventory.EquipWeapon(value);
	}

	// Attribute Area
	// defaultStats is static, and will be defined by the final class
	public abstract UnitStats unitStats { get; set; }
	public Guid ID { get => unitStats.ID; }
    public int STRENGTH {
		get => unitStats.STRENGTH;
		set => unitStats.STRENGTH = value;
	}
    public int HP {
		get => unitStats.HP;
		set {
			unitStats.HP = value;
			unitUI.UpdateHealthBar(unitStats.HP);
		}
	}
	public int MAXHP {
		get => unitStats.MAXHP;
		set {
			unitStats.MAXHP = value;
			unitUI.healthBar.maxPips = value;
		}
	}
    public int MOVE {
		get => unitStats.MOVE;
		set => unitStats.MOVE = value;
	}

	// derived stat: calculate from equipped weapon + modifiers
    public int _RANGE {
		get => equippedWeapon?.REACH ?? 1;
	}

	// cache the movement range for easier lookup later
	public MoveRange moveRange;
	public AttackRange attackRange;

	protected Dictionary<string, bool> optionAvailability = new Dictionary<string, bool>() {
		["Move"]   = true,
		["Attack"] = true
	};

	protected override void Awake() {
		base.Awake();

		unitUI = Instantiate(unitUIPrefab, this.transform);
		unitUI.healthBar.InitHealthBar(MAXHP);
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
	
	//void OnMouseOver() {
	//	Debug.DrawLine(boxCollider2D.bounds.min, boxCollider2D.bounds.max);
	//}
	void OnMouseOver() {	
		if (!ghosted) SetFocus(true);
	}
	void OnMouseExit() {
		if (selectionLock) return;
		SetFocus(false);
	}

	void Update() {
		// control all color information here, via polymorphic resolution
		var grid = GameManager.inst.GetActiveGrid();
		var mm = GameManager.inst.mouseManager;

		// Focus control: reset if applicable and highlight/focus
		if (!selectionLock && mm.prevMouseGridPos == gridPosition) SetFocus(false);
		if (!IsMoving() && mm.currentMouseGridPos == gridPosition) SetFocus(true);

		// Ghost control
		ghosted = false;
		if (!inFocus) {
			// each unit will check its own processes to see if it should be ghosted
			// having multiple senders, i.e. PathOverlayIso tiles and other Units, is difficult to keep track of

			// if there is any overlay that can be obscured:
			Vector3Int northPos = gridPosition + new Vector3Int(1, 1, 0);
			if (grid.GetOverlayAt(gridPosition) || grid.GetOverlayAt(northPos)) {
				ghosted = true;
			}
					
			// or, if there is a Unit with an active focus right behind
			if (((Unit)grid.OccupantAt(northPos))?.inFocus ?? false) {
				ghosted = true;
			}
		}

		// Color control
		// if no options are available, set your color again, just to be sure
		if (!optionAvailability.Values.Where(it => it == true).Any()) {
			spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f,
											 spriteRenderer.color.a);
		}
	}

	public void SetFocus(bool takeFocus) {
		if (takeFocus == inFocus) return;

		// only one unit can hold focus
		// force others to drop focus if their Y value is larger (unit is behind)
		inFocus = takeFocus;
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
		return gameObject.activeInHierarchy && HP > 0;
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
		unitUI.UpdateHealthBar(HP);
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
		})); 
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

		selectionLock = true;
		SetFocus(true);
	}

	public void OnDeselect() {
		var grid = GameManager.inst.tacticsManager.GetActiveGrid();
		//
		spriteRenderer.color = Color.white;
		moveRange?.ClearDisplay(grid);
		attackRange?.ClearDisplay(grid);

		selectionLock = false;
		SetFocus(false);
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
		// calculate damage here
		var outgoingDamage = (STRENGTH + equippedWeapon.MIGHT);
		var totalResistance = 0;

		other.SufferDamage(outgoingDamage - totalResistance);
	}

	public void SufferDamage(int incomingDamage) {
		HP -= incomingDamage;
		StartCoroutine(FlashColor(Utils.threatColorRed));
		StartCoroutine(Shake(0.075f));

		Debug.Log($"{this} suffered {incomingDamage}, {HP}/{MAXHP} health remaining.");

		if (HP < 1) { Die(); }
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
		var ogColor = spriteRenderer.color;

		float c = 1.0f;
		float rate = 0.005f;
		while (c > 0.0f) {
			var colorDiff = ogColor - (c * (ogColor - color));
			spriteRenderer.color = new Color(colorDiff.r, colorDiff.g, colorDiff.b, 1);
			c -= rate;
			rate *= 1.05f;
			yield return null;
		}
		spriteRenderer.color = ogColor;
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

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (animFlag) {
			yield return null;
		}
		VoidAction();
	}
}
