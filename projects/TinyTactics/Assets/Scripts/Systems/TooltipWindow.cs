using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

// contains general behavior about how to set text
// but also listens to the ScreenQuadrant of where the mouse is, and adjusts its RectTransform anchors accordingly
[RequireComponent(typeof(ReportScreenQuadrant))]
public class TooltipWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windowText_TMP;
    private RectTransform rectTransform;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable() {
        ChangeAnchorsBasedOnQuadrant(GetComponent<ReportScreenQuadrant>().CurrentQuadrant);
    }

    // this is a listener hooked up to the ReportScreenQuadrant event
    public void ChangeAnchorsBasedOnQuadrant(ReportScreenQuadrant.ScreenQuadrant quadrant) {
        switch (quadrant) {
            case ReportScreenQuadrant.ScreenQuadrant.LL:
                rectTransform.pivot = new Vector2(0, 0);
                break;

            case ReportScreenQuadrant.ScreenQuadrant.LR:
                rectTransform.pivot = new Vector2(1, 0);
                break;

            case ReportScreenQuadrant.ScreenQuadrant.UL:
                rectTransform.pivot = new Vector2(0, 1);
                break;

            case ReportScreenQuadrant.ScreenQuadrant.UR:
                rectTransform.pivot = new Vector2(1, 1);
                break;

            default:
                Debug.LogError($"{quadrant} is not a valid ScreenQuadrant");
                break;
        }
    }

    public void SetInfo(string toolTip) {
        windowText_TMP.SetText(toolTip);

        // finally, deal with Unity wonkiness
		foreach (ContentSizeFitter csf in GetComponentsInChildren<ContentSizeFitter>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
		}
    }
}