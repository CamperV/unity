using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Extensions;

public class ActionButton : MonoBehaviour,
                            IPointerUpHandler, IPointerDownHandler, IPointerClickHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Image image;

    private Action callbackAction;

    private bool _active = false;
    public bool active {
        get => _active;
        set {
            _active = value;
            image.color = image.color.WithTint( (value) ? 1.0f : 0.5f );
        }
    }

    void Awake() {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        callbackAction?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!active) return;
        transform.localScale *= 1.2f;
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (!active) return;
        image.color = image.color.WithTint(0.5f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!active) return;
        image.color = image.color.WithTint(1.0f);
        transform.localScale = Vector3.one;
    }
    public void OnPointerUp(PointerEventData eventData) { 
        if (!active) return;
        image.color = image.color.WithTint(1.0f);
    }

    // callbacks assigned to these buttons must have no arguments or return values
    public void BindCallback(Action action) {
        callbackAction = action;
    }

    public void UpdateTint(float tint) {
        image.color = image.color.WithTint(tint);
    }
}
