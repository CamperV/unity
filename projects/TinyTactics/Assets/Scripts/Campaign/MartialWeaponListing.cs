using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MartialWeaponListing : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public Image weaponTypeBackground;

    public TextMeshProUGUI mightValue;
    public TextMeshProUGUI accuracyValue;
    public TextMeshProUGUI weightValue;

    public void SetWeaponInfo(MartialWeapon weapon) {
        nameValue.SetText(weapon.displayName);
        weaponTypeBackground.color = weapon.color;

        mightValue.SetText($"{weapon.weaponStats.MIGHT}");
        accuracyValue.SetText($"{weapon.weaponStats.ACCURACY}");
        weightValue.SetText($"{weapon.weaponStats.WEIGHT}");
    }
}
