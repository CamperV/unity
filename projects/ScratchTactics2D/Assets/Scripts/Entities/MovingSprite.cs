using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MovingSprite : MovingGridObject
{	
	public static MovingSprite ConstructWith(Vector3 pos, Sprite sprite) {
		GameObject go = new GameObject("MovingSprite");
		go.transform.position = pos;
		go.AddComponent<SpriteRenderer>();
		go.AddComponent<SpriteAnimator>();
		go.AddComponent<MovingSprite>();

		go.GetComponent<SpriteRenderer>().sprite = sprite;
		go.GetComponent<SpriteRenderer>().sortingLayerName = "Tactics Entities";
		return go.GetComponent<MovingSprite>();
	}

	public void SendToAndDestruct(Vector3 toPosition) {
		StartCoroutine( GetComponent<SpriteAnimator>().SmoothMovement(toPosition, 1f) );
		StartCoroutine( GetComponent<SpriteAnimator>().ExecuteAfterMoving(() => Destroy(gameObject)) );
	}
}
