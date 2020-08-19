using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
	private Sprite playerSprite;
	private SpriteRenderer spriteRenderer;
	
	private Animator animator;

	
    // Start is called before the first frame update
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
        playerSprite = SpritesResourcesLoader.GetPlayerSprite();
		spriteRenderer.sprite = playerSprite;
    }
	
	protected override void Start() {
		//animator = GetComponent<Animator>();
		base.Start();
		Debug.Log("Started");
	}

    // Update is called once per frame
    void Update() {
		if (Input.GetKeyDown("left")) 	AttemptMove<Component>(-1,  0);
		if (Input.GetKeyDown("right")) 	AttemptMove<Component>( 1,  0);
		if (Input.GetKeyDown("up")) 	AttemptMove<Component>( 0,  1);
		if (Input.GetKeyDown("down")) 	AttemptMove<Component>( 0, -1);
    }
	
	public void ResetPosition() {
		transform.position = worldGrid.Tile2RealPos(new Vector3Int(1, 1, 0));
		Debug.Log("Player position is " + transform.position);
	}
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;
	}
	
	protected override void OnBlocked<T>(T component) {
		//Enemy hitEnemy = component as Enemy;
		//hitEnemy.Blink();
		//animator.SetTrigger("wut");
		Debug.Log("Collided with " + component);
	}
	
	// collision with any other Collider2D
	private void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("Collided with " + other);
	}
}
