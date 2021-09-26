using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitUI : MonoBehaviour
{
    // this UnitUI must be bound to a particular unit, and access its information
    [HideInInspector] public Unit boundUnit;
    [HideInInspector] public List<UnitUIElement> boundElements;

    [HideInInspector] public float persistentAlpha = 0.0f;

    // UI Elements to collect, scale, etc
    public TextUI textUIPrefab;
    public Emblem weaponTypeEmblemPrefab;
    
    [HideInInspector] public MiniHealthBar healthBar;
    [HideInInspector] public Emblem weaponTypeEmblem;

    void Awake() {
        boundElements = new List<UnitUIElement>();

        // spawn UnitUIElements
        healthBar = GetComponentsInChildren<MiniHealthBar>()[0];
        healthBar.BindUI(this);

        weaponTypeEmblem = Instantiate(weaponTypeEmblemPrefab, transform);
        weaponTypeEmblem.BindUI(this);
    }

    public void BindUnit(Unit unit) {
        Debug.Assert(boundUnit == null);
        boundUnit = unit;
    }

    public void UpdateHealthBar() {
        healthBar.UpdateBar(boundUnit._HP, boundUnit.VITALITY, persistentAlpha);
    }

    public void UpdateHealthBarThenFade() {
        healthBar.UpdateBar(boundUnit._HP, boundUnit.VITALITY, 1.0f);

        // set the transparency for a while, then fade down
        StartCoroutine(Utils.DelayedExecute(3.0f, () => {
			StartCoroutine(healthBar.FadeDown(1.0f));
		}));
    }

    public void UpdateWeaponEmblem() {
        weaponTypeEmblem.SetSprite( Emblem.FromWeapon(boundUnit.equippedWeapon) );
    }

    public void SetTransparency(float alpha) {
        persistentAlpha = alpha;

        foreach (UnitUIElement el in boundElements) {
            el.UpdateTransparency(persistentAlpha);
        }
    }

    public void DisplayDamageMessage(string message, bool emphasize = false) {
        // valid options are: damage taken, damage done? miss, crit
        TextUI textUI = Instantiate(textUIPrefab, transform);
        textUI.transform.position += new Vector3(0, boundUnit.spriteHeight*1.0f, 0);

        // determine if emphasis is necessary
        if (emphasize) {
            textUI.Bold();
            textUI.SetColor(Constants.threatColorRed);
            message = $"!!! {message} !!!";
            textUI.SetScale(1.5f);
        }
        textUI.SetText(message);

        // animate the motion here
        // this will destory the textUI gameObject
        StartCoroutine(textUI.FloatAway(2.0f, boundUnit.spriteHeight * 0.5f));
    }
}