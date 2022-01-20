using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponListing : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public Image weaponTypeBackground;

    public TextMeshProUGUI mightValue;
    public TextMeshProUGUI accuracyValue;
    public TextMeshProUGUI weightValue;

    public void SetWeaponInfo(Weapon weapon) {
        nameValue.SetText(weapon.displayName);
        weaponTypeBackground.color = weapon.color;

        mightValue.SetText($"{weapon.GetComponent<WeaponStats>().MIGHT}");
        accuracyValue.SetText($"{weapon.GetComponent<WeaponStats>().ACCURACY}");
        weightValue.SetText($"{weapon.GetComponent<WeaponStats>().WEIGHT}");
    }
}
