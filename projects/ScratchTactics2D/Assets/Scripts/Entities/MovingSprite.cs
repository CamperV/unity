using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MovingSprite : MonoBehaviour
{	
	public static MovingSprite ConstructWith(Vector3 pos, Sprite sprite, string layer) {
		GameObject go = new GameObject("MovingSprite");
		go.transform.position = pos;
		go.AddComponent<SpriteRenderer>();
		go.AddComponent<SpriteAnimator>();
		go.AddComponent<MovingSprite>();

		go.GetComponent<SpriteRenderer>().sprite = sprite;
		go.GetComponent<SpriteRenderer>().sortingLayerName = layer;
		return go.GetComponent<MovingSprite>();
	}

	public void SendToAndDestruct(Vector3 toPosition, float fixedTime) {
		StartCoroutine( GetComponent<SpriteAnimator>().SmoothMovement(toPosition, fixedTime) );
		StartCoroutine( GetComponent<SpriteAnimator>().FadeDown(fixedTime) );
		StartCoroutine( GetComponent<SpriteAnimator>().ExecuteAfterMoving(() => Destroy(gameObject)) );
	}
}
