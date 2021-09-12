using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

public abstract class EnemyArmy : Army, IVisible
{
	public abstract List<List<string>> spawnablePods { get; }
	public sealed override string armyTag { get => "EnemyArmy"; }

	public static int globalMoveThreshold { get => Constants.standardTickCost*3; }

	// for visualization debug
	private int _ID = -1;
	public int ID {
		get => _ID;
		set {
	    	GetComponentsInChildren<TextMeshPro>()[0].SetText(value.ToString());
			_ID = value;
		}
	}

	private RuntimeAnimatorController defaultAnimator;
	private RuntimeAnimatorController hiddenAnimator;

	// IVisible definitions
	private Enum.VisibleState _visible;
	public Enum.VisibleState visible {
		get => _visible;
		set {
			_visible = value;
			switch (value) {
				case Enum.VisibleState.visible:
				case Enum.VisibleState.partiallyObscured:
					spriteRenderer.color = Color.white.WithAlpha(1.0f);
					animator.runtimeAnimatorController = defaultAnimator;
					// debug text
					GetComponentsInChildren<TextMeshPro>()[0].color = GetComponentsInChildren<TextMeshPro>()[0].color.WithAlpha(1.0f);
					break;
				case Enum.VisibleState.obscured:
					spriteRenderer.color = Color.white.WithAlpha(1.0f);
 					animator.runtimeAnimatorController = hiddenAnimator;
					 // debug text
					 GetComponentsInChildren<TextMeshPro>()[0].color = GetComponentsInChildren<TextMeshPro>()[0].color.WithAlpha(0.0f);
					break;
				case Enum.VisibleState.hidden:
					spriteRenderer.color = Color.white.WithAlpha(0.0f);
					// debug text
					GetComponentsInChildren<TextMeshPro>()[0].color = GetComponentsInChildren<TextMeshPro>()[0].color.WithAlpha(0.0f);
					break;
			}
		}
	}

	// we update the visibility twice:
	// 1) when the player changes its FOV, update all
	// 2) when an IVisible moves
	[HideInInspector] public override Vector3Int gridPosition {
		get => _gridPosition;

		// make sure you also update FOV when moving
		protected set {
			_gridPosition = value;

			FieldOfView fov = GlobalPlayerState.army.fov;
			if (fov?.field.ContainsKey(_gridPosition) ?? false) {
				visible = fov.field[_gridPosition];
			} else {
				visible = Enum.VisibleState.hidden;
			}
		}
	}

	// OVERRIDABLES
	public virtual int detectionRange { get => 1; }

	public override String ToString() {
		return base.ToString() + $" [{ID}]";
	}
	// OVERRIDABLES

	// OverworldEneies have a special property: tickPool
	// when the PlayerArmy moves, they add ticks to all active entities controlled by EnemyArmyController
	// the enemies must check their pool for when they want to move. This will allow enemies to sometimes move
	// multiple times in reference to the player - likewise, the player can move morethan once before an
	// enemy can theoretically move
	private int tickPool { get; set; }
	
	public Enum.EnemyArmyState state;
	private EnemyArmyController parentController { get => GameManager.inst.enemyArmyController; }

	public abstract void OnHit();
	public abstract void OnAlert();
	
    void Awake() {
		animator = GetComponent<Animator>();
		//
		tickPool = 0;

		// devug visualization
		GetComponentsInChildren<MeshRenderer>()[0].sortingLayerName = "Overworld Entities";
		GetComponentsInChildren<MeshRenderer>()[0].sortingOrder = 0;

		defaultAnimator = animator.runtimeAnimatorController;
		hiddenAnimator = Resources.Load<RuntimeAnimatorController>("Icons/UnknownArmy");

        int podIndex = Random.Range(0, spawnablePods.Count);
		List<string> pod = spawnablePods[podIndex];
		PopulateBarracksFromTags(pod);
    }
	
    protected void Start() {
		state = Enum.EnemyArmyState.idle;
    }
	
	public void TakeIdleAction() {
		return;
	}
	
	public bool InDetectionRange(FlowField flowField) {
		return gridPosition.ManhattanDistance(flowField.origin) <= detectionRange;
	}

	public void AddTicks(int ticks) {
		tickPool += ticks;
	}

	protected void SpendTicks(int ticks) {
		tickPool -= (int)(ticks / moveSpeed);
	}

	// for future expandability:
	// right now, the closest enemy acts first
	// but in the future, maybe stamina effects will happen
	public float CalculateInitiative() {
		int md = gridPosition.ManhattanDistance(GlobalPlayerState.army.gridPosition);

		float directionScore = 0.0f;
		switch (gridPosition - GlobalPlayerState.army.gridPosition) {
			case Vector3Int v when v.Equals(Vector3Int.up):
				directionScore = 0.0f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.right):
				directionScore = 0.1f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.down):
				directionScore = 0.2f;
				break;
			case Vector3Int v when v.Equals(Vector3Int.left):
				directionScore = 0.3f;
				break;
		}

		return (float)md + directionScore;
	}
	
	public bool FollowField(FlowField flowField, Component target) {		
		List<Vector3Int> potentialMoves = new List<Vector3Int>() {
			gridPosition + Vector3Int.up,		// N
			gridPosition + Vector3Int.right,	// E
			gridPosition + Vector3Int.down,		// S
			gridPosition + Vector3Int.left		// W
		};
		var grid = GameManager.inst.overworld;
		
		// don't move until within detection range
		int minCost = flowField.field[gridPosition];
		Vector3Int selectedMove = gridPosition + Vector3Int.zero;
		
		// select your next move
		// if move remains unselected, remain still (.zero)
		foreach(Vector3Int move in potentialMoves) {		
			// otherwise, we must
			// a) make sure the move is in the flowfield
			if (flowField.field.ContainsKey(move)) {
				if (parentController.currentEnemyPositions.Contains(move))
					continue;
				
				if(flowField.field[move] < minCost) {
					minCost = flowField.field[move];
					selectedMove = move;
				}
			}
		}
		
		// here, we check to see if we can even make this move (point pool)
		// NOTE: it's important that we select our move before checking if we can move
		// if we don't, you might have a little back and forth on a road
		//
		// Also, standardize the "attack" bump. Don't make them move into a square
		bool moveSuccess = false;
		var tickCost = (selectedMove == flowField.origin) ? Constants.standardTickCost : grid.TerrainAt(selectedMove).tickCost;
		if (tickPool > 0 && tickCost <= tickPool) {
			Vector3Int nextStep = StepToPosition(selectedMove, 1);
			moveSuccess = GridMove(nextStep.x, nextStep.y);
			//
			SpendTicks(tickCost);
		}
		
		// return var is used to send a keepAlive signal to the phase
		// if you've depleted your pool, send a false
		// if you were able to move and the pool is not depleted, keep it alive
		return moveSuccess && tickPool >= Constants.standardTickCost;
	}

	public override bool GridMove(int xdir, int ydir) {
		var overworld = GameManager.inst.overworld;

		Component occupant;
		bool success = AttemptGridMove(xdir, ydir, overworld, out occupant);

		if (occupant?.MatchesType(typeof(PlayerArmy)) ?? false) {
			InitiateBattle();
		}
		return occupant == null && success;
	}

	public virtual bool CanAttackPlayer() {
		return tickPool > 0 && gridPosition.AdjacentTo(GlobalPlayerState.army.gridPosition);
	}

	public void InitiateBattle() {
		// entities can spend their entire remaining tickPool to attack a player
		SpendTicks(tickPool);
		BumpTowards(GlobalPlayerState.army.gridPosition, GameManager.inst.overworld);

		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => {
			GameManager.inst.EnterBattleState();

			// programmatically load in a TacticsGrid that matches what we need
			Terrain thisTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(GlobalPlayerState.army.gridPosition);
			Battle.CreateActiveBattle(GlobalPlayerState.army, this, playerTerrain, thisTerrain, Enum.Phase.enemy);
		}));
	}

	public void JoinBattle() {
		// we don't need this function to start the battle phase, let the EnemyArmyController release back and Resume the battle
		// however, it will now resume with a new enemy joining
		SpendTicks(tickPool);
		BumpTowards(GlobalPlayerState.army.gridPosition, GameManager.inst.overworld);

		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			Terrain thisTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Battle.active.AddParticipant(this, thisTerrain);
		}));
	}

	// Unit things:
	public override void PopulateBarracksFromTags(List<string> pod) {
		foreach (string unitClassTag in pod) {
			Guid _ID = Guid.NewGuid();
			string _unitName = $"{unitClassTag} Enemy!Jeremy {Random.Range(0, 101)}";
			UnitState defaultState = UnitClass.GenerateDefaultState(_ID, _unitName, unitClassTag);

			// now save the unit in our barracks
			EnlistUnit(defaultState);
		}
	}
}
