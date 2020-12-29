using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    public HealthBar healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    void Awake() {
        // spawn health bar
        healthBar = Instantiate(healthBarPrefab, transform);
		healthBar.transform.parent = transform;
    }
}
