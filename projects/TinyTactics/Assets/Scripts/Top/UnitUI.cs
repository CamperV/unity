using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private MiniBar_UI healthBar;
    [SerializeField] private SegBar_UI damageReductionBar;
    [SerializeField] private MiniBar_UI breakBar;
    [SerializeField] private StatusBarUI statusBar;

    [SerializeField] private DisplayValue_UI healthValueDisplay;
    [SerializeField] private GameObject targetIndicator;
    [SerializeField] private GameObject deathIndicator;

    private Unit boundUnit;

    void Awake() {
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        healthBar.AttachTo(boundUnit);
        damageReductionBar.AttachTo(boundUnit);
        breakBar.AttachTo(boundUnit);
        statusBar.AttachTo(boundUnit);

        healthValueDisplay.AttachTo(boundUnit);
    }

    public void PreviewDamage(int minDamage, int maxDamage, bool isAggressor = false) {
        // if you're going to take damage
        if (maxDamage > 0) {
            healthBar.PreviewDamage(minDamage, maxDamage);
        }

        // if you're DEFINITELY gonna die
        if (minDamage >= boundUnit.statSystem.CURRENT_HP) {
            deathIndicator.gameObject.SetActive(true);
        }

        // these are not technically mutually exclusive, but they occupy the same zone for now
        // } else if (!isAggressor) {
        //     targetIndicator.gameObject.SetActive(true);
        // }
    }

    public void RevertPreview() {
        healthBar.RevertPreview();
        // targetIndicator.gameObject.SetActive(false);
        deathIndicator.gameObject.SetActive(false);
    }
}
