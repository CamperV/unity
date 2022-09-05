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

    public override void DisplayComboAttacks(List<ComboAttack> comboAttacks) {
        Clear();

        if (comboAttacks.Count > 0) {
            container.SetActive(true);
            
            foreach (ComboAttack comboAttack in comboAttacks) {
                PortraitComboAttackItem item = Instantiate(itemPrefab, container.transform);
                item.SetPortrait(comboAttack.unit.portraitSprite);
                item.SetText($"+{comboAttack.damage}");
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