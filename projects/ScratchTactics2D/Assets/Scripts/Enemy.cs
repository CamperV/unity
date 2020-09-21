using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	public int moveSpeed = 1;
	public int pathRange = 50;
	
	public MovingObjectPath pathToPlayer; // use to cache and not recalculate every frame	
	
	public static Enemy Spawn(Enemy prefab) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileReal();
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
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
		pathToPlayer = new MovingObjectPath(gridPosition);
    }
	
	public bool MoveTowardsPlayer() {	
		// chase player given the coordinates
		if (GameManager.inst.enemyManager.HasPlayerMoved() || !pathToPlayer.IsValid()) {
			pathToPlayer.Clear();	// also un-draws it
			pathToPlayer = MovingObjectPath.GetPathTo(gridPosition, GameManager.inst.player.gridPosition, pathRange);
			//pathToPlayer.DrawPath();
		}

		Vector3Int nextStep = ToPosition(pathToPlayer.PopNext(gridPosition), moveSpeed);
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
	
	protected override bool AttemptGridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {}
}
