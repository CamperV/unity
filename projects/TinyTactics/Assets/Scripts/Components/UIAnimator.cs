using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Extensions;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
	private CanvasGroup canvasGroup;
	private Image image;

	void Awake() {
		canvasGroup = GetComponent<CanvasGroup>();
		image = GetComponent<Image>();
	}

	private int _animationStack;
	public int animationStack {
		get => _animationStack;
		set {
			Debug.Assert(value > -1);
			_animationStack = value;
		}
	}
	public bool isAnimating => animationStack > 0;

	public Queue<Action> actionQueue = new Queue<Action>();
	private Coroutine processActionQueue;
	
	// fashioned as Func<bool> for WaitUntil convenience
	public bool DoneAnimating() => !isAnimating;
	public bool EmptyQueue() => actionQueue.Count == 0 && processActionQueue == null;
	public bool DoneAnimatingAndEmptyQueue() => DoneAnimating() && EmptyQueue();

	public void ClearStacks() {
		_animationStack = 0;
	}

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (isAnimating) {
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

	public IEnumerator FadeDown(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		float canvasOriginalAlpha = canvasGroup.alpha;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			canvasGroup.alpha = Mathf.Lerp(canvasOriginalAlpha, 0f, timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}

	public IEnumerator FadeDown(float fixedTime, Action PostExec) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		float canvasOriginalAlpha = canvasGroup.alpha;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			canvasGroup.alpha = Mathf.Lerp(canvasOriginalAlpha, 0f, timeRatio);
			yield return null;
		}

		//
		animationStack--;
		PostExec();
	}

	public IEnumerator FadeUp(float fixedTime) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		float canvasOriginalAlpha = canvasGroup.alpha;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			canvasGroup.alpha = Mathf.Lerp(canvasOriginalAlpha, 1f, timeRatio);
			yield return null;
		}

		//
		animationStack--;
	}

	public IEnumerator FadeUp(float fixedTime, Action PostExec) {
		animationStack++;
		//

		float timeRatio = 0.0f;
		float canvasOriginalAlpha = canvasGroup.alpha;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			canvasGroup.alpha = Mathf.Lerp(canvasOriginalAlpha, 1f, timeRatio);
			yield return null;
		}

		//
		animationStack--;
		PostExec();
	}

    public IEnumerator Flash(Color color_0, Color color_1) {
        float fixedTime = 0.5f;
		float timeRatio = 0.0f;

        while (true) {
            // to dim
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(color_0, color_1, timeRatio);
                yield return null;
            }

            // and then back up to normal color
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(color_1, color_0, timeRatio);
                yield return null;
            }
        }
    }

	public void TriggerFadeDown(float fadeTime) {
		StopAllCoroutines();
		StartCoroutine( FadeDown(fadeTime) );
	}

	public void TriggerFadeDownDisable(float fadeTime) {
		StopAllCoroutines();
		StartCoroutine( FadeDown(fadeTime, () => gameObject.SetActive(false)) );
	}
	
	public void TriggerFadeUp(float fadeTime) {
		StopAllCoroutines();
		StartCoroutine( FadeUp(fadeTime) );
	}

	public void TriggerFadeUpEnable(float fadeTime) {
		StopAllCoroutines();
		StartCoroutine( FadeUp(fadeTime, () => gameObject.SetActive(true)) );
	}
}