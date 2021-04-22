using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

public class OverworldEnemyBase : OverworldEntity
{
	// for visualization debug
	private int _ID = -1;
	public int ID {
		get => _ID;
		set {
	    	GetComponentsInChildren<TextMeshPro>()[0].SetText(value.ToString());
			_ID = value;
		}
	}

	// OVERRIDABLES
	public virtual int detectionRange { get { return 1; } }
	public override HashSet<Type> spawnable {
		get {
			return new HashSet<Type>() { typeof(GrassWorldTile) };
		}
	}

	public override String ToString() {
		return base.ToString() + $" [{ID}]";
	}
	// OVERRIDABLES

	// OverworldEneies have a special property: tickPool
	// when the OverworldPlayer moves, they add ticks to all active entities controlled by EnemyController
	// the enemies must check their pool for when they want to move. This will allow enemies to sometimes move
	// multiple times in reference to the player - likewise, the player can move morethan once before an
	// enemy can theoretically move
	private int tickPool { get; set; }
	
	public Enum.EnemyState state;
	public MovingObjectPath pathToPlayer; // use to cache and not recalculate every frame

	private EnemyController parentController { get => GameManager.inst.enemyController; }
	
	// will only spawn into a spawnable tile
	public static OverworldEnemyBase Spawn(OverworldEnemyBase prefab, int ID = -1) {
		OverworldEnemyBase enemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		var grid = GameManager.inst.worldGrid;
		
		// this will auto-check occupancy
		enemy.ResetPosition( grid.RandomTileWithinType(enemy.spawnable) );
		grid.UpdateOccupantAt(enemy.gridPosition, enemy);

		enemy.ID = ID;
		return enemy;
	}
	
    protected override void Awake() {
		base.Awake();
		//
		tickPool = 0;

		// devug visualization
		GetComponentsInChildren<MeshRenderer>()[0].sortingLayerName = "Overworld Entities";
		GetComponentsInChildren<MeshRenderer>()[0].sortingOrder = 0;
    }
	
    protected void Start() {
		state = Enum.EnemyState.idle;
    }
	
	public void TakeIdleAction() {
		return;
	}
	
	public bool InDetectionRange(FlowField flowField) {
		return gridPosition.ManhattanDistance(flowField.origin) <= detectionRange;
	}

	public void AddTicks(int ticks) {
		tickPool += ticks;
		Debug.Log($"{this} received {ticks} ticks [{tickPool}]");
	}

	protected void SpendTicks(int ticks) {
		tickPool -= (int)(ticks / moveSpeed);
		Debug.Log($"{this} spent {ticks} [{tickPool}]");
	}

	// for future expandability:
	// right now, the closest enemy acts first
	// but in the future, maybe stamina effects will happen
	public float CalculateInitiative() {
		int md = gridPosition.ManhattanDistance(GameManager.inst.player.gridPosition);

		float directionScore = 0.0f;
		switch (gridPosition - GameManager.inst.player.gridPosition) {
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
		var grid = GameManager.inst.worldGrid;
		
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
		var tickCost = (selectedMove == flowField.origin) ? Constants.standardTickCost : grid.GetTileAt(selectedMove).cost;
		
		if (tickPool > 0 && tickCost <= tickPool) {
			Vector3Int nextStep = ToPosition(selectedMove, 1);
			moveSuccess = GridMove(nextStep.x, nextStep.y);
			//
			SpendTicks(tickCost);
		}
		
		// return var is used to send a keepAlive signal to the phase
		// if you've depleted your pool, send a false
		// if you were able to move and the pool is not depleted, keep it alive
		return moveSuccess;
	}
		
	public void OnHit() { return; }
	
	public void Alert() {
		animator.SetTrigger("SkeletonAlert");
	}
	
	// let's be real... "entity" is just "player"
	// in the future, I'd like to be able to orchestrate battles b/w NPCs
	// but that's in the future. Change terminology now, to confuse less
	public override void OnBlocked<T>(T component) {
		Debug.Log($"{this} would have strode right into {this}");
	}

	public virtual bool CanAttackPlayer() {
		//return tickPool >= Constants.standardTickCost && gridPosition.AdjacentTo(GameManager.inst.player.gridPosition);
		return tickPool > 0 && gridPosition.AdjacentTo(GameManager.inst.player.gridPosition);
	}

	public void InitiateBattle() {
		// entities can spend their entire remaining tickPool to attack a player
		SpendTicks(tickPool);
		BumpTowards(GameManager.inst.player.gridPosition, GameManager.inst.worldGrid);

		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			WorldTile thisTile = GameManager.inst.worldGrid.GetTileAt(gridPosition) as WorldTile;
			WorldTile playerTile = GameManager.inst.worldGrid.GetTileAt(GameManager.inst.player.gridPosition) as WorldTile;
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(GameManager.inst.player, this, playerTile, thisTile, Enum.Phase.enemy);
		}));
	}

	public void JoinBattle() {
		// we don't need this function to start the battle phase, let the EnemyController release back and Resume the battle
		// however, it will now resume with a new enemy joining
		SpendTicks(tickPool);
		BumpTowards(GameManager.inst.player.gridPosition, GameManager.inst.worldGrid);

		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			WorldTile thisTile = GameManager.inst.worldGrid.GetTileAt(gridPosition) as WorldTile;			
			GameManager.inst.tacticsManager.AddToActiveBattle(this, thisTile);
		}));
	}
}
