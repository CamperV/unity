using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    // this UnitUI must be bound to a particular unit, and access its information
    [HideInInspector] public Unit boundUnit;
    [HideInInspector] public List<UnitUIElement> boundElements;

    [HideInInspector] public float persistentAlpha = 0.0f;

    // UI Elements to collect, scale, etc
    public HealthBar healthBarPrefab;
    public TextUI textUIPrefab;
    
    [HideInInspector] public HealthBar healthBar;
    [HideInInspector] public TextUI weaponDisplay;

    void Awake() {
        boundElements = new List<UnitUIElement>();

        // spawn UnitUIElements
        healthBar = Instantiate(healthBarPrefab, transform);
        healthBar.BindUI(this);
        boundElements.Add(healthBar);

        //weaponDisplay = Instantiate(textUIPrefab, transform);
        //weaponDisplay.BindUI(this);
        //boundElements.Add(weaponDisplay);
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
			StartCoroutine(healthBar.FadeDown(1.0f));
            healthBar.transparencyLock = false;
		}));
    }

    public void SetTransparency(float alpha) {
        persistentAlpha = alpha;

        foreach (UnitUIElement el in boundElements) {
            el.UpdateTransparency(persistentAlpha);
        }
    }

    public void DisplayDamageMessage(string message) {
        // valid options are: damage taken, damage done? miss, crit
        // do not register this to the UI, we don't want the transform to move with our unitUI
        TextUI textUI = Instantiate(textUIPrefab, transform);
        textUI.SetText(message);
        textUI.transparencyLock = true;

        StartCoroutine(Utils.DelayedExecute(1.5f, () => {
            StartCoroutine(textUI.FadeDown(1.0f));
            textUI.transparencyLock = false;
			Destroy(textUI);
		}));
    }
}