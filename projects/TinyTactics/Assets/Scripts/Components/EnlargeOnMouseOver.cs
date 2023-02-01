using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnlargeOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleMultiplier;
    private Vector3 originalScale;

    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = scaleMultiplier*originalScale;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = originalScale;
}