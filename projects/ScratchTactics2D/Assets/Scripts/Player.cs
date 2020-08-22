using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;

    // Start is called before the first frame update
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetPlayerSprite();;
    }
	
	protected override void Start() {
		//animator = GetComponent<Animator>();
		base.Start();
	}

    // Update is called once per frame
    void Update() {
		if (!GameManager.inst.playerPhase) return;
		
		if (Input.GetKeyDown("left")) 	AttemptMove<Enemy>(-1,  0);
		if (Input.GetKeyDown("right")) 	AttemptMove<Enemy>( 1,  0);
		if (Input.GetKeyDown("up")) 	AttemptMove<Enemy>( 0,  1);
		if (Input.GetKeyDown("down")) 	AttemptMove<Enemy>( 0, -1);
    }
	
	public void ResetPosition() {
		transform.position = GameManager.inst.worldGrid.Tile2RealPos(new Vector3Int(1, 1, 0));
	}
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;
	}
	
	protected override void OnBlocked<T>(T component) {
		Enemy hitEnemy = component as Enemy;
		//hitEnemy.Blink();
		//animator.SetTrigger("wut");
		Debug.Log("BLOCKED with " + hitEnemy);
	}
	
	// collision with any other Collider2D
	private void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log("Collided with " + other);
	}
}
