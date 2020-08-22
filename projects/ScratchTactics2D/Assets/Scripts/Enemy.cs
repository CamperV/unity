using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
	private SpriteRenderer spriteRenderer;
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetEnemySprite();
    }
	
    // Start is called before the first frame update
    void Start() {
        base.Start();
		Debug.Log(">>> I LIVE! @" + transform.position);
    }

    // Update is called once per frame
    void Update() {
        
    }
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;
	}
	
	protected override void OnBlocked<T>(T component) {
		//Enemy hitEnemy = component as Enemy;
		//hitEnemy.Blink();
		//animator.SetTrigger("wut");
		Debug.Log("Enemy collided with " + component);
	}
}
