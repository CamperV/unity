using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class PerkListing : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image perkImage;
    public Image perkImageMatte;

    public GameObject mouseOverLabel;
    public TextMeshProUGUI nameValue;

    void Start() {

    }
    
    // update UI stuff
    public void SetPerkInfo(PerkData perkData) {
        Debug.Log($"Grabbing sprite from PerkData {perkData}");
        perkImage.sprite = perkData.sprite;
        perkImageMatte.color = (1f*perkData.belongsToArchetype.color).WithAlpha(1f);

        mouseOverLabel.SetActive(true);
        nameValue.SetText(perkData.perkName);
        foreach (var rt in mouseOverLabel.GetComponentsInChildren<RectTransform>()) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
        mouseOverLabel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOverLabel.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData) {
        mouseOverLabel.SetActive(false);
    }
}
