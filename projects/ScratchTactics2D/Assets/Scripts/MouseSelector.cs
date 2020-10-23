using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelector : MovingObject
{
	private Dictionary<Enum.GameState, Sprite> spriteOptions;
	private SpriteRenderer spriteRenderer;
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		
		spriteOptions = new Dictionary<Enum.GameState, Sprite>() {
			[Enum.GameState.overworld] = ResourceLoader.GetSprite("select_overlay_tile"),
			[Enum.GameState.battle]	   = ResourceLoader.GetSprite("select_overlay_iso_tile")
		};
	}
	
    void Start() {
        base.Start();
    }

    void Update() {
		// update current sprite
        spriteRenderer.sprite = spriteOptions[GameManager.inst.gameState];
    }
	
	public void MoveTo(Vector3 endpoint) {
		if (crtMovingFlag) StopCoroutine(crtMovement);
		crtMovement = StartCoroutine(FastMovement(endpoint));
	}
	
	private IEnumerator FastMovement(Vector3 endpoint) {
		float sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
		float snapFactor = 0.01f;
		
		float speedFactor;
		
		crtMovingFlag = true;
		while (sqrRemainingDistance > snapFactor) {	
			speedFactor = (250.0f * Time.deltaTime);

			Vector3 newPos = Vector3.MoveTowards(rigidbody2D.position, endpoint, speedFactor);
			rigidbody2D.MovePosition(newPos);
			sqrRemainingDistance = (transform.position - endpoint).sqrMagnitude;
			
			yield return null; // waits for a new frame
		}
		
		// after the while loop is broken:
		rigidbody2D.MovePosition(endpoint);
		crtMovingFlag = false;
	}
	
	protected override void OnBlocked<T>(T component) {}
}
