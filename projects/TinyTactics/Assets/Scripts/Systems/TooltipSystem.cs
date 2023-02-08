using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class TooltipSystem : MonoBehaviour
{
    // this class exists to be an active GameObject in the scene,
    // to have children register themselves to various flags
    // this allows for nice, separable components
    public static TooltipSystem inst = null; // enforces singleton behavior

    [SerializeField] private GameObject infoPanel;
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
    }

    public void DisplayTooltip(string toolTip) {
        infoPanel.SetActive(true);
        infoPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(toolTip);
    }

    public void HideTooltip() {
        infoPanel.SetActive(false);
    }
}