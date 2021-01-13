using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public abstract class UnitUIElement : MonoBehaviour
{
    protected int _animationStack;
	protected int animationStack {
		get => _animationStack;
		set {
			Debug.Assert(value > -1);
			_animationStack = value;
		}
	}

    [HideInInspector] UnitUI parentUI;
    public Unit boundUnit { get => parentUI?.boundUnit ?? null; }

    public bool transparencyLock = false;

    public void BindUI(UnitUI UI) {
        Debug.Assert(parentUI == null);
        parentUI = UI;
    }

	//
	// Animation Coroutines
	//
	public bool IsAnimating() {
		return animationStack > 0;
	}

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (animationStack > 0) {
			yield return null;
		}
		VoidAction();
	}

    public virtual IEnumerator FadeDown(float fixedTime) {
        animationStack++;
		//

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            UpdateTransparency(1.0f - timeRatio);
			yield return null;
		}

        //
		animationStack--;
	}

	// not relative to time: shake only 3 times, wait a static amt of time
	public IEnumerator Shake(float radius) {
		animationStack++;
		//

		var ogPosition = transform.position;
		for (int i=0; i<3; i++) {
			transform.position = transform.position + (Vector3)Random.insideUnitCircle*radius;
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		transform.position = ogPosition;

		//
		animationStack--;
	}

    public abstract void UpdateTransparency(float alpha);
}