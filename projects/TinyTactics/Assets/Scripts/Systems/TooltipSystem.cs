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

    [SerializeField] private TooltipWindow infoPanel;
    private UIAnimator windowAnimator;
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}

		windowAnimator = infoPanel.GetComponent<UIAnimator>();
    }

    public void DisplayTooltip(string toolTip) {
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(toolTip);

		// cancel any fade downs, start fade up
		windowAnimator.TriggerFadeUp(0.1f);
    }

    public void HideTooltip() {
        // cancel any fade ups, start fade down
		// after fade down, set active false
		windowAnimator.TriggerFadeDownDisable(0.1f);
    }

    public void TryHideTooltip() {
		if (infoPanel.gameObject.activeInHierarchy) HideTooltip();
    }
}