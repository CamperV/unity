using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	public bool phaseActionTaken { get; set; }

    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetSprite("yellow_skull");
		
		phaseActionTaken = false;
    }
	
	protected override void Start() {
		base.Start();
	}

    // Update is called once per frame
    void Update() {
		if (!MyPhase()) return;
		
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.player && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		// phaseAction possibilities
		if 		(Input.GetKeyDown("left"))  { AttemptMove<Enemy>(-1,  0); }
		else if (Input.GetKeyDown("right")) { AttemptMove<Enemy>( 1,  0); }
		else if (Input.GetKeyDown("up"))    { AttemptMove<Enemy>( 0,  1); }
		else if (Input.GetKeyDown("down"))  { AttemptMove<Enemy>( 0, -1); }
		else 								{ return false; } // no input taken
		// other possibilities
		//
		
		// if we've reached this code, the phaseAction has been taken
		// basically, any time Input is recorded a phaseAction is taken
		// if no Input happens, we auto-return before this line
		return true;
	}
		
	public void ResetPosition() {
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(new Vector3Int(1, 1, 0));
	}
	
	public Vector3Int GetGridPosition() {
		return GameManager.inst.worldGrid.Real2GridPos(transform.position);
	}
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Enemy hitEnemy = component as Enemy;
		hitEnemy.OnHit();
	}
	
	// collision with any other Collider2D
	private void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log("Collided with " + other);
	}
}
