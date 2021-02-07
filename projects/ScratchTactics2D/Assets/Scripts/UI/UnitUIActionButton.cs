using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class UnitUIActionButton : UnitUIElement
{
    public string location;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer backgroundSpriteRenderer;
    public float spriteWidth { get => spriteRenderer.bounds.size.x; }

    private Action callbackAction;

    private bool _active = false;
    public bool active {
        get => _active;
        set {
            _active = value;
            spriteRenderer.color = spriteRenderer.color.WithTint( (value) ? 1.0f : 0.5f );
        }
    }

    private bool triggerInvoke = false;

    public static UnitUIActionButton Spawn(Transform parent, UnitUIActionButton prefab, Sprite sprite) {
        var button = Instantiate(prefab, parent);
        button.spriteRenderer.sprite = sprite;
        return button;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1]; // [1] b/c this will find its own SR
    }

    void OnMouseDown() {
        if (!active) return;
        spriteRenderer.color = spriteRenderer.color.WithTint(0.5f);
        transform.localScale = Vector3.one;
        triggerInvoke = true;
    }

    void OnMouseUp() {
        if (!active) return;
        if (triggerInvoke) {
            spriteRenderer.color = spriteRenderer.color.WithTint(1.0f);
            transform.localScale = Vector3.one;
            callbackAction?.Invoke();
        }
        triggerInvoke = false;
    }

    void OnMouseEnter() {
        if (!active) return;
        transform.localScale *= 1.2f;
    }

    void OnMouseExit() {
        triggerInvoke = false;
        transform.localScale = Vector3.one;
    }

    public override void UpdateTransparency(float alpha) {
        Color c = spriteRenderer.color;
        spriteRenderer.color = c.WithAlpha(alpha);

        // update the background as well
        // but never let its color be more than  0.5f
       backgroundSpriteRenderer.color = c.WithAlpha(alpha / 2.0f);
    }

    public void UpdateTint(float tint) {
        spriteRenderer.color = spriteRenderer.color.WithTint(tint);
    }

    // callbacks assigned to these buttons must have no arguments or return values
    public void BindCallback(Action action) {
        callbackAction = action;
    }
}
