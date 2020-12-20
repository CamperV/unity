using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OverworldEnemyBase : OverworldEntity
{
	// OVERRIDABLES
	public virtual int moveSpeed { get { return 1; } }
	public virtual int pathRange { get { return 50; } }
	public virtual int detectionRange { get { return 5; } }
	public virtual HashSet<Type> untraversable {
		get {
			return new HashSet<Type>() { typeof(MountainWorldTile) };
		}
	}
	// OVERRIDABLES
	//
	
	public Enum.EnemyState state;
	public MovingObjectPath pathToPlayer; // use to cache and not recalculate every frame	
	
	// will never spawn into an untraversable tile
	public static OverworldEnemyBase Spawn(OverworldEnemyBase prefab) {
		OverworldEnemyBase enemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileExceptTypeReal(enemy.untraversable);
		//
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {
		base.Awake();
    }
	
    protected void Start() {
		state = Enum.EnemyState.idle;
    }
	
	public void TakeIdleAction() {
		return;
	}
	
	public bool InDetectionRange(FlowField flowField) {
		if (flowField.field.ContainsKey(gridPosition)) {
			return flowField.field[gridPosition] <= detectionRange;
		} else {
			return false;
		}
	}
	
	public bool FollowField(FlowField flowField, Component target) {		
		List<Vector3Int> potentialMoves = new List<Vector3Int>() {
			gridPosition + Vector3Int.up,		// N
			gridPosition + Vector3Int.right,	// E
			gridPosition + Vector3Int.down,		// S
			gridPosition + Vector3Int.left		// W
		};
		
		// don't move until within detection range
		int minCost = flowField.field[gridPosition];
		Vector3Int selectedMove = gridPosition + Vector3Int.zero;
		
		// select your next move
		// if move remains unselected, remain still (.zero)
		foreach(Vector3Int move in potentialMoves) {
			// either check the tag or type of occupant
			// this check allows enemies to "move" into untraversables
			// only if they have a Player in them
			var occupant = GameManager.inst.worldGrid.OccupantAt(move);
			if (occupant != null) {
				if(occupant.GetType() == target.GetType()) {
					selectedMove = move;
					break; // early
				} else continue;
			}
			
			// otherwise, we must
			// a) make sure the move is in the flowfield
			if (flowField.field.ContainsKey(move)) {			
				//
				if(flowField.field[move] < minCost) {
					minCost = flowField.field[move];
					selectedMove = move;
				}
			}
		}
		
		Vector3Int nextStep = ToPosition(selectedMove, moveSpeed);
		var moveSuccess = GridMove(nextStep.x, nextStep.y);
		
		return moveSuccess;
	}
	
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
		
	public void OnHit() {
		animator.SetTrigger("SkeletonAlert");
	}
	
	public void Alert() {
		animator.SetTrigger("SkeletonAlert");
	}
	
	public override bool GridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir, GameManager.inst.worldGrid);
	}
	
	// let's be real... "entity" is just "player"
	// in the future, I'd like to be able to orchestrate battles b/w NPCs
	// but that's in the future. Change terminology now, to confuse less
	public override void OnBlocked<T>(T component) {
		OverworldEntity player = component as OverworldEntity;
		if (player.GetType() == typeof(OverworldEnemyBase)) {
			return;
		}
		
		// programmatically load in a TacticsGrid that matches what we need
		var thisTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(gridPosition);
		var playerTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(player.gridPosition);
		
		GameManager.inst.EnterBattleState();
		var battleParticipants = new List<OverworldEntity>() { player, this };
		var battleTiles = new List<WorldTile>(){ playerTile, thisTile };
		GameManager.inst.tacticsManager.CreateActiveBattle(battleParticipants, battleTiles, Enum.Phase.enemy);
	}
}
