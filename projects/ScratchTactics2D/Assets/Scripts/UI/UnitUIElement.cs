using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public abstract class UnitUIElement : MonoBehaviour
{
    protected bool animFlag;

    [HideInInspector] UnitUI parentUI;
    public Unit boundUnit { get => parentUI?.boundUnit ?? null; }

    public bool transparencyLock = false;

    public void BindUI(UnitUI UI) {
        Debug.Assert(parentUI == null);
        parentUI = UI;
    }

    public virtual IEnumerator FadeDown(float fixedTime) {
        animFlag = true;
		float timeRatio = 0.0f;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            UpdateTransparency(1.0f - timeRatio);
			yield return null;
		}

        animFlag = false;
	}

	// not relative to time: shake only 3 times, wait a static amt of time
	public IEnumerator Shake(float radius) {
		var ogPosition = transform.position;
		for (int i=0; i<3; i++) {
			transform.position = transform.position + (Vector3)Random.insideUnitCircle*radius;
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}
		transform.position = ogPosition;
	}

	public IEnumerator ExecuteAfterAnimating(Action VoidAction) {
		while (animFlag) {
			yield return null;
		}
		VoidAction();
	}

    public bool IsAnimating() {
        return animFlag;
    }

    public abstract void UpdateTransparency(float alpha);
}