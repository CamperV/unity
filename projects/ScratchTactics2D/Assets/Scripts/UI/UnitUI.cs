using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [HideInInspector] public float persistentAlpha = 0.0f;

    public HealthBar healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    public WeaponDisplay weaponDisplayPrefab;
    [HideInInspector] public WeaponDisplay weaponDisplay;

    void Awake() {
        // spawn health bar
        healthBar = Instantiate(healthBarPrefab, transform);
		healthBar.transform.parent = transform;

        weaponDisplay = Instantiate(weaponDisplayPrefab, transform);
        weaponDisplay.transform.parent = transform;
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

    public void UpdateWeaponDisplay(string val) {
        weaponDisplay.SetCurrentWeapon(val);
    }

    public void SetTransparency(float alpha) {
        persistentAlpha = alpha;

		healthBar.UpdateBarTransparency(persistentAlpha);
    }
}