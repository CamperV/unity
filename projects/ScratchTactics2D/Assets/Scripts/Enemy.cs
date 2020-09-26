using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	public int moveSpeed = 1;
	public int pathRange = 50;
	public int sightRange = 10;	// in WorldTiles?
	public HashSet<Type> sightBlockers = new HashSet<Type>() { typeof(MountainWorldTile) };
	public Enum.EnemyState state;
	
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
	
	public bool InLineOfSight(Vector3Int target) {
		if ((int)Vector3Int.Distance(gridPosition, target) > sightRange) return false;
		
		Debug.Log("target in range, calculating");
		// else, in detection range:
		// draw a ray from pos to target
		MovingObjectPath sightRay = MovingObjectPath.GetRayTo(gridPosition, target);
		HashSet<Vector3Int> mySet = new HashSet<Vector3Int>();
		foreach (var k in sightRay.path.Keys) {
			mySet.Add(k);
		}
		mySet.Add(sightRay.end);
		GameManager.inst.worldGrid.HighlightTiles(mySet, Color.red);
		
		// check the ray for sight-blockers
		bool blocked = false;
		//Vector3Int currPos = sightRay.start;
		/*while (currPos != sightRay.end) {
			if (sightBlockers.Contains(GameManager.inst.worldGrid.GetWorldTileAt(currPos).GetType())) {
				Debug.Log("Sight is blocked by object @ " + currPos);
				blocked = true;
				break;
			}
			currPos = sightRay.Next(currPos);
		}*/
		
		return !blocked;
	}
	
	public void TakeIdleAction() {
		return;
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
