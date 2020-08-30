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
		Vector3Int chaseVec = ClampVec(GameManager.inst.player.GetGridPosition() - GetGridPosition(), moveSpeed);
												 
		// randomly decide b/w axes if its a tie
		if (Mathf.Abs(chaseVec.x) == Mathf.Abs(chaseVec.y)) {
			int randInt = Random.Range(0, 2);
			if (randInt == 0) chaseVec.x = 0;
			else chaseVec.y = 0;
		}
		
		AttemptMove<Player>(chaseVec.x, chaseVec.y);
		
		phaseActionTaken = true;
		return phaseActionTaken;
	}
	
	public Vector3Int GetGridPosition() {
		return GameManager.inst.worldGrid.Real2GridPos(transform.position);
	}
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Debug.Log("Enemy collided with " + component);
	}
	
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}

}
