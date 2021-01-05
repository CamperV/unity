using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
	private Text currentWeapon;
	
    void Awake() {
        currentWeapon = GameObject.Find("CurrentWeapon").GetComponent<Text>();
    }

    public void SetCurrentWeapon(string val) {
        currentWeapon.text = val;
    }
}