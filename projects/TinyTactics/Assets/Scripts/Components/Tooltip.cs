using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Extensions;

public interface ITooltip
{
    string GetTooltip();
}

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool useOverride;
    [SerializeField] private string tooltipOverride;

    // if you're connected to anything that implements the ITooltip interface, 
    // use their function for describing your tool tip
    // otherwise, use the default override
    // for now, just get the first toolTipper you find
    private string GetTooltip() {
        if (!useOverride)
            return GetComponent<ITooltip>()?.GetTooltip() ?? "DEFAULT";
        else
            return tooltipOverride;
    }

    public void OnPointerEnter(PointerEventData eventData) => TooltipSystem.inst.DisplayTooltip(GetTooltip());
    public void OnPointerExit(PointerEventData eventData) => TooltipSystem.inst.HideTooltip();
    void OnDisable() => TooltipSystem.inst.HideTooltip();
}
