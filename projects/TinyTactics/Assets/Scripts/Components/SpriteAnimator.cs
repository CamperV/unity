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
	public static bool skipMovement = false;

	public Action<Vector3> PositionUpdater;
	private SpriteRenderer spriteRenderer;

	private int _animationStack;
	public int animationStack {
		get => _animationStack;
		set {
			Debug.Assert(value > -1);
			_animationStack = value;
		}
	}
	public bool isAnimating { get => animationStack > 0; }

	protected int _movementStack;
	public int movementStack {
		get => _movementStack;
		set {
			Debug.Assert(value > -1);
			_movementStack = value;
		}
	}
	public bool isMoving { get => movementStack > 0; }

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();

		// else if you don't have this component, construct a default Updater
		PositionUpdater = v => transform.position = v;
	}

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (isAnimating) {
			yield return null;
		}
		VoidAction();
	}

	public IEnumerator ExecuteAfterMoving(Action VoidAction) {
		while (isMoving) {
			yield return null;
		}
		VoidAction();
	}

	public IEnumerator FadeDown(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}

	public IEnumerator FadeDownThen(float fixedTime, Action PostExec) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			yield return null;
		}

		//
		animationStack--;
		PostExec();
	}

	public IEnumerator FadeDownThenDestroy(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f - timeRatio);
			yield return null;
		}

		//
		animationStack--;
		Destroy(gameObject);
	}

	public IEnumerator FadeUp(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = spriteRenderer.color.WithAlpha(timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}

	public IEnumerator TweenColor(Color color, float fixedTime) {
		animationStack++;
		//
		var ogColor = spriteRenderer.color;

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			spriteRenderer.color = Color.Lerp(ogColor, color, timeRatio).WithAlpha(1.0f);
			yield return null;
		}

		//
		animationStack--;
	}
	
	public IEnumerator FlashColor(Color color) {
		animationStack++;
		//
		var ogColor = spriteRenderer.color;

		float fixedTime = 1.0f;
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			var colorDiff = ogColor - ((1.0f - timeRatio) * (ogColor - color));
			spriteRenderer.color = colorDiff.WithAlpha(1.0f);

			yield return null;
		}
		spriteRenderer.color = ogColor;

		//
		animationStack--;
	}

	// not relative to time: shake only 3 times, wait a static amt of time
	public IEnumerator Shake(float radius) {
		animationStack++;
		movementStack++;
		//

		// I would love to do this in a one-liner...
		// (Select, etc) but due to Unity's choices, you cant' ToList a safeTransform for its enumerated children
		// this should preserve order...?
		var ogPosition = transform.position;
		var childOgPositions = new List<Vector3>();
		foreach (Transform child in transform) childOgPositions.Add(child.position);
		int index;

		for (int i=0; i<3; i++) {
			Vector3 offset = (Vector3)Random.insideUnitCircle*radius;
			transform.position = ogPosition + offset;

			// reverse offset all children, so only the main Unit shakes
			index = 0;
			foreach (Transform child in transform) {
				child.position = childOgPositions[index] - offset;
				index++;
			}
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		transform.position = ogPosition;
		index = 0;
		foreach (Transform child in transform) {
			child.position = childOgPositions[index];
			index++;
		}

		//
		animationStack--;
		movementStack--;
	}
	
	public IEnumerator SmoothMovement(Vector3 endpoint, float _fixedTime = -1f) {
		if (skipMovement) {
			PositionUpdater(endpoint);
			yield break;
		}
		movementStack++;
        //

		float timeStep = 0.0f;
		Vector3 startPos = transform.position;

        float fixedTime = (_fixedTime == -1f) ? fixedTimePerTile : _fixedTime;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / fixedTime);
			PositionUpdater(Vector3.Lerp(startPos, endpoint, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		PositionUpdater(endpoint);
		movementStack--;
	}
	
	// this coroutine performs a little 'bump' when you can't move
	public IEnumerator SmoothBump(Vector3 endpoint, float distanceScale) {
		if (skipMovement) {
			PositionUpdater(transform.position);
			yield break;
		}
		movementStack++;

		Vector3 startPos = transform.position;
		Vector3 peakPos = startPos + (endpoint - transform.position)/distanceScale;
		
		float timeStep = 0.0f;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			PositionUpdater(Vector3.Lerp(startPos, peakPos, timeStep));
			yield return null;
		}

		// now for the return journey
		timeStep = 0.0f;
		while (timeStep < 1.0f)  {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			PositionUpdater(Vector3.Lerp(peakPos, startPos, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		PositionUpdater(startPos);
		movementStack--;
	}

	public IEnumerator SmoothMovementPath(Path<GridPosition> path, Grid grid) {
		if (skipMovement) {
			PositionUpdater(grid.CellToWorld(path.end));
			yield break;
		}
		movementStack++;
        //

		Vector3 realNextPos = transform.position;
		foreach (var nextPos in path.Unwind()) {
			realNextPos = grid.CellToWorld(nextPos);

			float timeStep = 0.0f;
			Vector3 startPos = transform.position;

			while (timeStep < 1.0f) {
				timeStep += (Time.deltaTime / fixedTimePerTile);

				PositionUpdater(Vector3.Lerp(startPos, realNextPos, timeStep));
				yield return null;
			}
		}
		PositionUpdater(realNextPos);

        //
		movementStack--;
	}
}