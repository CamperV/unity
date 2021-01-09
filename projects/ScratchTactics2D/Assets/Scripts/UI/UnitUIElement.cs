using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class UnitUIElement : MonoBehaviour
{
    [HideInInspector] UnitUI parentUI;
    public Unit boundUnit { get => parentUI?.boundUnit ?? null; }

    public bool transparencyLock = false;

    public void BindUI(UnitUI UI) {
        Debug.Assert(parentUI == null);
        parentUI = UI;
    }

    public virtual IEnumerator FadeDown(float fixedTime) {
		float timeRatio = 0.0f;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            UpdateTransparency(1.0f - timeRatio);
			yield return null;
		}
	}

    public abstract void UpdateTransparency(float alpha);
}