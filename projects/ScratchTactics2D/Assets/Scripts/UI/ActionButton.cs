using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class ActionButton : UnitUIElement
{
    public string location;
    public SpriteRenderer spriteRenderer;

    public static ActionButton Spawn(Transform parent, ActionButton prefab, Sprite sprite, string loc) {
        var button = Instantiate(prefab, parent);
        button.spriteRenderer.sprite = sprite;
        button.location = loc;
        return button;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        transform.localScale *= 0.25f;
        transform.position += new Vector3(0, spriteRenderer.bounds.size.y*0.75f, 0);
        //
        switch (location) {
            case "N":
            case "W":
                transform.position -= new Vector3(spriteRenderer.bounds.size.x*1.25f, 0, 0);
                break;
            case "S":
            case "E":
                transform.position += new Vector3(spriteRenderer.bounds.size.x*1.25f, 0, 0);
                break;
            default:
                Debug.Log($"{location} is an invalid location setting for ActionButton");
                break;
        }
    }

    void OnMouseDown() {
        Debug.Log($"This is where I'd activate the registered callback");
    }

    void OnMouseEnter() {
        transform.localScale *= 1.2f;
    }

    void OnMouseExit() {
        transform.localScale /= 1.2f;
    }

    public override void UpdateTransparency(float alpha) {
        spriteRenderer.color = spriteRenderer.color.WithAlpha(alpha);
    }
}
