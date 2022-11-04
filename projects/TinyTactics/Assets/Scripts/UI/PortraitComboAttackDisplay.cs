using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitComboAttackDisplay : UIComboAttackDisplay
{
    [SerializeField] private GameObject container;
    [SerializeField] private PortraitComboAttackItem itemPrefab;

    public override void DisplayComboAttacks(Engagement engagement) {
        Clear();

        if (engagement.comboAttacks.Count > 0) {
            container.SetActive(true);
            
            foreach (ComboAttack comboAttack in engagement.comboAttacks) {
                PortraitComboAttackItem item = Instantiate(itemPrefab, container.transform);
                item.SetPortrait(comboAttack.unit.portraitSprite);

                int finalDamage = Mathf.Clamp(comboAttack.damage - engagement.defense.damageReduction, 0, 99);
                if (finalDamage > 0) {
                    item.SetText($"+{finalDamage}");
                } else {
                    item.SetText($"<color=#ff0000>n/a</color>");
                }
            }
        }
    }

    private void Clear() {
        foreach (Transform child in container.transform) {
            Destroy(child.gameObject);
        }
        container.SetActive(false);
    }
}