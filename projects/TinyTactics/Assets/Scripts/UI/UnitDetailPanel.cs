using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDetailPanel : MonoBehaviour
{
    public Image portraitImage;
	public TextMeshProUGUI nameText;
	public Image weaponImage;
	public TextMeshProUGUI weaponNameText;
	
	// attributes
	public TextMeshProUGUI hpValue;
	public TextMeshProUGUI vitValue;
	public TextMeshProUGUI strValue;
	public TextMeshProUGUI dexValue;
	public TextMeshProUGUI refValue;
	public TextMeshProUGUI defValue;
	public TextMeshProUGUI movValue;
	
	// derived
	public TextMeshProUGUI atkValue;
	public TextMeshProUGUI hitValue;
	public TextMeshProUGUI avoValue;

	// perk listing
	public TextMeshProUGUI unitPerksValue;
}