using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class BasicTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mouseOverLabel;

    public void OnPointerEnter(PointerEventData eventData) => mouseOverLabel.SetActive(true);
    public void OnPointerExit(PointerEventData eventData) => mouseOverLabel.SetActive(false);
}
