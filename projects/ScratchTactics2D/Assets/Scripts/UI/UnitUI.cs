using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    // this UnitUI must be bound to a particular unit, and access its information
    [HideInInspector] public Unit boundUnit;
    [HideInInspector] public List<UnitUIElement> boundElements;

    [HideInInspector] public float persistentAlpha = 0.0f;

    public HealthBar healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    void Awake() {
        boundElements = new List<UnitUIElement>();

        // spawn UnitUIElements
        healthBar = Instantiate(healthBarPrefab, transform);
        healthBar.BindUI(this);
        boundElements.Add(healthBar);
    }

    public void BindUnit(Unit unit) {
        Debug.Assert(boundUnit == null);
		transform.parent = unit.transform;
        boundUnit = unit;
    }

    public void UpdateHealthBar(int val) {
        healthBar.UpdateBar(val, 1.0f);
        healthBar.transparencyLock = true;

        // set the transparency for a while, then fade down
        StartCoroutine(Utils.DelayedExecute(3.0f, () => {
			StartCoroutine(healthBar.FadeDown(0.05f));
            healthBar.transparencyLock = false;
		}));
    }

    public void SetTransparency(float alpha) {
        persistentAlpha = alpha;

		healthBar.UpdateBarTransparency(persistentAlpha);
    }
}