using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitComboAttackItem : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI text;

    public void SetPortrait(Sprite _portrait) {
        portrait.sprite = _portrait;
    }

    public void SetText(string message) {
        text.SetText($"<wave>{message}</wave>");
    }
}