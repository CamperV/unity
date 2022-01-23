using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class WakeOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    private float originalAlpha;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start() {
        canvasGroup.alpha = 0.5f;
    }

    public void OnPointerEnter(PointerEventData eventData) => canvasGroup.alpha = 1f;
    public void OnPointerExit(PointerEventData eventData) => canvasGroup.alpha = 0.5f;
}