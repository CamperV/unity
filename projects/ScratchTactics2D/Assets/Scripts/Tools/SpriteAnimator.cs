using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    // we want it to take X seconds to go over one tile
	public static float speedMultiplier = 1f;
	public static float fixedTimePerTile { get => 0.10f / speedMultiplier; }

	private SpriteRenderer spriteRenderer;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public static IEnumerator ExecuteAfterAnimating(IAnimatable target, Action VoidAction) {
		while (target.isAnimating) {
			yield return null;
		}
		VoidAction();
	}
	public static IEnumerator ExecuteAfterMoving(IMovable target, Action VoidAction) {
		while (target.isMoving) {
			yield return null;
		}
		VoidAction();
	}

	public static IEnumerator FadeDown(IAnimatable target, float fixedTime) {
		target.animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			target.spriteRenderer.color = target.spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			yield return null;
		}

		//
		target.animationStack--;
	}
	
	public static IEnumerator FlashColor(IAnimatable target, Color color) {
		target.animationStack++;
		//

		var ogColor = target.spriteRenderer.color;

		float fixedTime = 1.0f;
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			var colorDiff = ogColor - ((1.0f - timeRatio) * (ogColor - color));
			target.spriteRenderer.color = colorDiff.WithAlpha(1.0f);

			yield return null;
		}
		target.spriteRenderer.color = ogColor;

		//
		target.animationStack--;
	}

	// not relative to time: shake only 3 times, wait a static amt of time
	public static IEnumerator Shake(IAnimatable target, float radius) {
		target.animationStack++;
		//

		// I would love to do this in a one-liner...
		// (Select, etc) but due to Unity's choices, you cant' ToList a safeTransform for its enumerated children
		// this should preserve order...?
		var ogPosition = target.safeTransform.position;
		var childOgPositions = new List<Vector3>();
		foreach (Transform child in target.safeTransform) childOgPositions.Add(child.position);
		int index;

		for (int i=0; i<3; i++) {
			Vector3 offset = (Vector3)Random.insideUnitCircle*radius;
			target.safeTransform.position = ogPosition + offset;

			// reverse offset all children, so only the main Unit shakes
			index = 0;
			foreach (Transform child in target.safeTransform) {
				child.position = childOgPositions[index] - offset;
				index++;
			}
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		target.safeTransform.position = ogPosition;
		index = 0;
		foreach (Transform child in target.safeTransform) {
			child.position = childOgPositions[index];
			index++;
		}

		//
		target.animationStack--;
	}
}