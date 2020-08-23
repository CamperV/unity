using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetEnemySprite();
		
		animator = GetComponent<Animator>();
    }
	
    // Start is called before the first frame update
    void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        
    }
	
	protected override void AttemptMove<T>(int xdir, int ydir) {
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;
	}
	
	protected override void OnBlocked<T>(T component) {
		Debug.Log("Enemy collided with " + component);
	}
	
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}

}
