using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class DisplayValue_UI : MonoBehaviour
{	
    public enum RegistrationOptions {
        UpdateHPEvent,
        UpdatePoiseEvent,
        UpdateDamageReductionEvent
    };
    [SerializeField] private RegistrationOptions registerTo;

    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private int maxValue;

    public void AttachTo(Unit thisUnit) {
        switch (registerTo) {
            case RegistrationOptions.UpdateHPEvent:
                UpdateValue(thisUnit.statSystem.CURRENT_HP, thisUnit.statSystem.MAX_HP);
                thisUnit.statSystem.UpdateHPEvent += UpdateValue;
                break;

            case RegistrationOptions.UpdatePoiseEvent:
                UpdateValue(thisUnit.statSystem.CURRENT_POISE, thisUnit.statSystem.MAX_POISE);
                thisUnit.statSystem.UpdatePoiseEvent += UpdateValue;
                break;

            case RegistrationOptions.UpdateDamageReductionEvent:
                UpdateValue(thisUnit.statSystem.DAMAGE_REDUCTION, thisUnit.statSystem.DAMAGE_REDUCTION);
                thisUnit.statSystem.UpdateDamageReductionEvent += d => UpdateValue(d, 0);
                break;

            default:
                break;
        }
    }

    private void UpdateValue(int val, int max) {
        string final = (val <= maxValue) ? $"{val}" : "??";
        value.SetText(final);
    }
}
