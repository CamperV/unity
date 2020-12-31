using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    public float persistentAlpha = 0.0f;

    public HealthBar healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    void Awake() {
        // spawn health bar
        healthBar = Instantiate(healthBarPrefab, transform);
		healthBar.transform.parent = transform;
    }

    public void UpdateHealthBar(int val) {
        healthBar.UpdateBar(val, persistentAlpha);
    }

    public void SetTransparency(float alpha) {
        persistentAlpha = alpha;

		healthBar.UpdateBarTransparency(persistentAlpha);
    }
}