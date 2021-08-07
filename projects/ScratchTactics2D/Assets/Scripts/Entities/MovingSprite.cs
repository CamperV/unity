using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MovingSprite : MovingObject
{	
	public static MovingSprite ConstructWith(Vector3 pos, Sprite sprite) {
		GameObject go = new GameObject("MovingSprite");
		go.transform.position = pos;
		go.AddComponent<SpriteRenderer>();
		go.AddComponent<MovingSprite>();

		go.GetComponent<SpriteRenderer>().sprite = sprite;
		go.GetComponent<SpriteRenderer>().sortingLayerName = "Tactics Entities";
		return go.GetComponent<MovingSprite>();
	}

	public void SendToAndDestruct(Vector3 toPosition) {
		StartCoroutine( SmoothMovement(toPosition, _fixedTime: 1f) );
		StartCoroutine( ExecuteAfterMoving(() => Destroy(gameObject)) );
	}
}
