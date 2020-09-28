using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	public readonly int moveSpeed = 1;
	public readonly int pathRange = 50;
	public readonly int detectionRange = 5;
	public Enum.EnemyState state;
	
	public MovingObjectPath pathToPlayer; // use to cache and not recalculate every frame	
	
	// this class of Enemy cannot walk upon mountain tiles
	public static HashSet<Type> untraversable = new HashSet<Type>() { typeof(MountainWorldTile) };
	
	public static Enemy Spawn(Enemy prefab, HashSet<Type> untraversable) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileExceptTypeReal(untraversable);
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
		//
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
    }
	
    protected override void Start() {
        base.Start();
		//
		state = Enum.EnemyState.idle;
		pathToPlayer = new MovingObjectPath(gridPosition);
    }
	
	public bool MoveTowardsPosition(Vector3Int targetPosition) {	
		// chase player given the coordinates
		if (GameManager.inst.enemyManager.HasPlayerMoved() || !pathToPlayer.IsValid()) {
			pathToPlayer.Clear();	// also un-draws it
			pathToPlayer = MovingObjectPath.GetPathTo(gridPosition, targetPosition, pathRange);
			pathToPlayer.DrawPath();
		}

		Vector3Int nextStep = ToPosition(pathToPlayer.PopNext(gridPosition), moveSpeed);
		var moveSuccess = AttemptGridMove(nextStep.x, nextStep.y);
		
		return moveSuccess;
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
	
	public bool FollowField(FlowField flowField) {		
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
				if(occupant.GetType() == typeof(Player)) {
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
		var moveSuccess = AttemptGridMove(nextStep.x, nextStep.y);
		
		return moveSuccess;
	}
	
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
		
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}
	
	public void Alert() {
		animator.SetTrigger("EnemyFlash");
	}
	
	protected override bool AttemptGridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {}
}
