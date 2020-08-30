using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	[HideInInspector] public bool phaseActionTaken { get; set; }
	public int moveSpeed = 1;
	
	public static Enemy Spawn(Enemy prefab) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileReal();
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetSprite("yellow_skull_red_eyes");
		
		animator = GetComponent<Animator>();
		
		phaseActionTaken = false;
    }
	
    protected override void Start() {
        base.Start();
    }

    void Update() {}
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		// chase player given the coordinates
		Vector3Int chaseVec = SpeedVec(GameManager.inst.player.gridPosition - gridPosition, moveSpeed);
												 
		// randomly decide b/w axes if its a tie
		if (Mathf.Abs(chaseVec.x) == Mathf.Abs(chaseVec.y)) {
			int randInt = Random.Range(0, 3);
			if (randInt == 0) chaseVec.x = 0;
			else if (randInt == 1) chaseVec.y = 0;
			else { chaseVec.x = 0; chaseVec.y = 0; }	// sometimes, just don't move
		}
		
		AttemptGridMove(chaseVec.x, chaseVec.y);
		
		phaseActionTaken = true;
		return phaseActionTaken;
	}
	
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
	
	protected override void AttemptGridMove(int xdir, int ydir) {
		base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Debug.Log("Enemy collided with " + component);
	}
	
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}

}
