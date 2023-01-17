using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

public class SpriteAnimator : MonoBehaviour
{
    // we want it to take X seconds to go over one tile
	public static float speedMultiplier = 1f;
	public static float fixedTimePerTile { get => 0.10f / speedMultiplier; }
	public static bool skipMovement = false;

	public static Color InactiveColor => new Color(0.75f, 0.75f, 0.75f, 1.0f);

	public Action<Vector3> PositionUpdater;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Transform spriteTransform;
	private Color _originalColor;
	//
	public Sprite MainSprite => spriteRenderer.sprite;
	public Color MainColor => spriteRenderer.color;

	private int _animationStack;
	public int animationStack {
		get => _animationStack;
		set {
			Debug.Assert(value > -1);
			_animationStack = value;
		}
	}
	public bool isAnimating => animationStack > 0;

	protected int _movementStack;
	public int movementStack {
		get => _movementStack;
		set {
			Debug.Assert(value > -1);
			_movementStack = value;
		}
	}
	public bool isMoving => movementStack > 0;

	public Queue<Action> actionQueue = new Queue<Action>();
	private Coroutine processActionQueue;
	
	// fashioned as Func<bool> for WaitUntil convenience
	public bool DoneAnimating() => !isAnimating && !isMoving;
	public bool EmptyQueue() => actionQueue.Count == 0 && processActionQueue == null;
	public bool DoneAnimatingAndEmptyQueue() => DoneAnimating() && EmptyQueue();

	void Awake() {
		_originalColor = spriteRenderer.color;

		// else if you don't have this component, construct a default Updater
		PositionUpdater = v => transform.position = v;
	}

	public void ClearStacks() {
		_movementStack = 0;
		_animationStack = 0;
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

	public void QueueAction(Action VoidAction) {
		actionQueue.Enqueue(VoidAction);

		if (processActionQueue == null) {
			processActionQueue = StartCoroutine( ProcessActionQueue() );
		}
	}

	private IEnumerator ProcessActionQueue() {
		while (actionQueue.Count > 0) {
			yield return new WaitUntil(DoneAnimating);

			// once you're finished with the current animation, perform:
			Action Perform = actionQueue.Dequeue();
			Perform();
		}

		// when you've exhausted the queue:
		yield return new WaitUntil(DoneAnimating);
		processActionQueue = null;
	}

	///////////
	// COLOR //
	///////////
	public void SetColor(Color newColor) => spriteRenderer.color = newColor;
	public void SetPosition(Vector3 newPos) => spriteTransform.position = newPos;
    public void RevertColor() => SetColor(_originalColor);

    public void LerpInactiveColor(float lerpValue) {
		SetColor(Color.Lerp(spriteRenderer.color, InactiveColor, lerpValue));
	}

	public IEnumerator FadeDown(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			SetColor(spriteRenderer.color.WithAlpha(1.0f - timeRatio));
			yield return null;
		}

		//
		animationStack--;
	}

	public IEnumerator FadeDownAll(float fixedTime) {
		animationStack++;
		//
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		TextMeshPro[] textMeshes = GetComponentsInChildren<TextMeshPro>();
		CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();

		float[] rendererOriginalAlphas = new float[renderers.Length];
		for (int r = 0; r < rendererOriginalAlphas.Length; r++) {
			rendererOriginalAlphas[r] = renderers[r].color.a;
		}
		float canvasOriginalAlpha = canvasGroup.alpha;

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			// we check for null here, because some of the animators delete themselves, 
			// while their SpriteRenderers may have captured by GetComponentsInChildren at the
			// top of this function
			for (int rr = 0; rr < renderers.Length; rr++) {
				if (renderers[rr] != null) {
					float alpha = rendererOriginalAlphas[rr] * (1.0f - timeRatio);
					renderers[rr].color = renderers[rr].color.WithAlpha(alpha);
				}
			}

			foreach (var tm in textMeshes) {
				tm.color = tm.color.WithAlpha(1.0f - timeRatio);
			}

			if (canvasGroup != null) {
				canvasGroup.alpha = Mathf.Lerp(canvasOriginalAlpha, 0f, timeRatio);
			}
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
			SetColor(spriteRenderer.color.WithAlpha(1.0f - timeRatio));
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
			SetColor(spriteRenderer.color.WithAlpha(1.0f - timeRatio));
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
			SetColor(spriteRenderer.color.WithAlpha(timeRatio));
			yield return null;
		}

		//
		animationStack--;
	}
	
	public IEnumerator FlashColorThenRevert(Color color) {
		animationStack++;
		//
		
		float fixedTime = 1.0f;
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);

			var colorDiff = _originalColor - ((1.0f - timeRatio) * (_originalColor - color));
			SetColor(colorDiff.WithAlpha(1.0f));

			yield return null;
		}

		// finally
		RevertColor();
		
		//
		animationStack--;
	}

	////////////
	// MOTION //
	////////////

	// not relative to time: shake only 3 times, wait a static amt of time
	public IEnumerator Shake(float radius, int numberOfShakes) {
		animationStack++;
		//
		Vector3 startPos = spriteTransform.position;

		for (int i = 0; i < numberOfShakes; i++) {
			Vector3 offset = (Vector3)Random.insideUnitCircle*radius;
			SetPosition(startPos + offset);

			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}

		SetPosition(startPos);

		//
		animationStack--;
	}

	public IEnumerator SmoothCosX(float freq, float amplitude, float phase, float fixedTime) {
		animationStack++;
        //

		float timeStep = 0.0f;
		Vector3 startPos = spriteTransform.position;

		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / fixedTime);
			
			Vector3 xComponent = amplitude*(1f-timeStep) * (Mathf.Cos( (freq*Time.time) + phase)) * Vector3.right;
			SetPosition(Vector3.Lerp(spriteTransform.position, startPos + xComponent, timeStep));

			yield return null;
		}
		
		// after the while loop is broken:
		SetPosition(startPos);
		animationStack--;
	}


	public IEnumerator BumpTowards<T>(T target, IGrid<T> surface, float distanceScale = 5.0f) where T : struct {
		yield return SmoothBump(surface.GridToWorld(target), distanceScale);
	}
	
	// this coroutine performs a little 'bump' when you can't move
	public IEnumerator SmoothBump(Vector3 endpoint, float distanceScale) {
		if (skipMovement) yield break;
		animationStack++;

		Vector3 startPos = spriteTransform.position;
		Vector3 peakPos = startPos + (endpoint - spriteTransform.position)/distanceScale;
		
		float timeStep = 0.0f;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			SetPosition(Vector3.Lerp(startPos, peakPos, timeStep));
			yield return null;
		}

		// now for the return journey
		timeStep = 0.0f;
		while (timeStep < 1.0f)  {
			timeStep += (Time.deltaTime / (0.5f*fixedTimePerTile) );
			SetPosition(Vector3.Lerp(peakPos, startPos, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		SetPosition(startPos);
		animationStack--;
	}


	// this coroutine performs a little 'bump' when you can't move
	public IEnumerator SmoothBumpRandom(float radius) {
		if (skipMovement) yield break;
		animationStack++;

		float timeScale = 0.5f*fixedTimePerTile;

		// get random location for spike
		Vector3 startPos = spriteTransform.position;
		Vector3 peakPos = startPos + (Vector3)Random.insideUnitCircle*radius;
		
		float timeStep = 0.0f;
		while (timeStep < 1.0f) {
			timeStep += (Time.deltaTime / (timeScale/4f) );
			SetPosition(Vector3.Lerp(startPos, peakPos, timeStep));
			yield return null;
		}

		// now for the return journey
		timeStep = 0.0f;
		while (timeStep < 1.0f)  {
			timeStep += (Time.deltaTime / (4f*timeScale) );
			SetPosition(Vector3.Lerp(peakPos, startPos, timeStep));
			yield return null;
		}
		
		// after the while loop is broken:
		SetPosition(startPos);
		animationStack--;
	}
	
	////////////////////
	// UNIT MAP MOVER //
	////////////////////
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

	public IEnumerator SmoothMovementGrid<T>(T target, IGrid<T> surface, float _fixedTime = -1f) where T : struct {
		Vector3 endpoint = surface.GridToWorld(target);

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

	public IEnumerator SmoothMovementPath<T>(Path<T> path, IGrid<T> surface) where T : struct, IEquatable<T> {
		if (skipMovement) {
			PositionUpdater(surface.GridToWorld(path.End));
			yield break;
		}
		movementStack++;
        //

		Vector3 realNextPos = transform.position;
		foreach (T nextPos in path.Unwind()) {
			realNextPos = surface.GridToWorld(nextPos);

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