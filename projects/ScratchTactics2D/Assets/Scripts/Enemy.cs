using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	[HideInInspector] public bool phaseActionTaken { get; private set; }
	public int moveSpeed = 1;
	public int pathRange = 50;
	
	public MoverPath pathToPlayer; // use to cache and not recalculate every frame	
	
	public static Enemy Spawn(Enemy prefab) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileReal();
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {	
		animator = GetComponent<Animator>();
		
		phaseActionTaken = false;
    }
	
    protected override void Start() {
        base.Start();
		//
		pathToPlayer = new MoverPath(gridPosition);
    }
	
	public bool MyPhase() {
		return GameManager.inst.phaseManager.currentPhase == PhaseManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public void TriggerPhase() {
		phaseActionTaken = false;
	}
	
	public bool TakePhaseAction() {
		// chase player given the coordinates
		if (GameManager.inst.enemyManager.HasPlayerMoved() || !pathToPlayer.IsValid()) {
			pathToPlayer.Clear();	// also un-draws it
			pathToPlayer = MoverPath.GetPathTo<Player>(gridPosition, GameManager.inst.player.gridPosition, pathRange);
			//pathToPlayer.DrawPath();
		}

		Vector3Int nextStep = ToPosition(pathToPlayer.PopNext(gridPosition), moveSpeed);		
		AttemptGridMove(nextStep.x, nextStep.y);
		
		phaseActionTaken = true;
		return phaseActionTaken;
	}
	
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
		
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}
	
	protected override void AttemptGridMove(int xdir, int ydir) {
		base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {}
}
