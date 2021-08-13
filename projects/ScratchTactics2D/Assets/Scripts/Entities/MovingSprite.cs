using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MovingSprite : MonoBehaviour
{	
	private SpriteAnimator spriteAnimator;
	private SpriteRenderer spriteRenderer;

	public static MovingSprite ConstructWith(Vector3 pos, Sprite sprite, string layer, Transform parentTransform) {
		GameObject go = new GameObject($"MovingSprite_{sprite.name}");
		go.transform.position = pos;
		go.transform.SetParent(parentTransform);
		
		go.AddComponent<SpriteRenderer>();
		go.AddComponent<SpriteAnimator>();
		go.AddComponent<MovingSprite>();
		//go.AddComponent<SpriteTrailBehavior>();
		go.AddComponent<SpriteDirectionalBlurBehavior>();

		go.GetComponent<SpriteRenderer>().sprite = sprite;
		go.GetComponent<SpriteRenderer>().sortingLayerName = layer;
		return go.GetComponent<MovingSprite>();
	}

	void Awake() {
		spriteAnimator = GetComponent<SpriteAnimator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		// With URP enabled, the default material is Sprite-Lit-Default
		// since we have no illumination in this project, gotta find the old one...
		spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
	}

	// this update loop should preseve all sprites real sorting order when animating
	void Update() {
		//spriteRenderer.sortingOrder = (int)(-100f * (transform.position.y-transform.position.z)); // This prioritizes height
		spriteRenderer.sortingOrder = (int)(-100f * transform.position.y);	// This prioritizes obscurability, things like trees
	}
	
	public void SendToAndDestroy(Vector3 toPosition, float fixedTime) {
		StartCoroutine( spriteAnimator.SmoothMovement(toPosition, fixedTime) );

		// these execute sequentially
		// StartCoroutine( spriteAnimator.FadeDown(fixedTime/2f) );
		// StartCoroutine( spriteAnimator.ExecuteAfterAnimating(() => {
		// 	StartCoroutine( spriteAnimator.FadeUp(fixedTime/2f) );
		// }) );
		// Color gray = 0.85f * GetComponent<SpriteRenderer>().color;
		// StartCoroutine( spriteAnimator.TweenColor(gray, fixedTime/2f) );
		// StartCoroutine( spriteAnimator.ExecuteAfterAnimating(() => {
		// 	StartCoroutine( spriteAnimator.TweenColor(Color.white, fixedTime/2f) );
		// }) );

		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => Destroy(gameObject)) );
	}

	public void SendToAndDestroyArc(Vector3 toPosition, Vector3 relativePivot, float fixedTime) {
		StartCoroutine( spriteAnimator.SmoothMovementArc(toPosition, relativePivot, fixedTime) );
		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => Destroy(gameObject)) );
	}
}
