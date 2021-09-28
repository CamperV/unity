using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class Emblem : UnitUIElement
{	
    private SpriteRenderer spriteRenderer;
	private float spriteHeight { get => spriteRenderer.bounds.size.y; }

    public static Sprite FromWeapon(Weapon weapon) {
        foreach (string tag in weapon.tags) {
            switch (tag) {
                case "slash":
                    return ResourceLoader.GetSprite("Icons/slash_emblem");
                case "pierce":
                    return ResourceLoader.GetSprite("Icons/pierce_emblem");
                case "strike":
                    return ResourceLoader.GetSprite("Icons/strike_emblem");
                case "missile":
                    return ResourceLoader.GetSprite("blank_portrait");
            }
        }

        // if none found that fits:
        return ResourceLoader.GetSprite("blank_portrait");
    }

    public static Color ColorFromWeapon(Weapon weapon) {
        foreach (string tag in weapon.tags) {
            switch (tag) {
                case "slash":
                    return Color.red;
                case "pierce":
                    return Color.blue;
                case "strike":
                    return Color.green;
                case "missile":
                    return Color.grey;
            }
        }

        // if none found that fits:
        return Color.magenta;
    }

	void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void UpdateTransparency(float alpha) {
        spriteRenderer.color = spriteRenderer.color.WithAlpha(1.0f);
    }

    public void SetSprite(Sprite sp) {
        spriteRenderer.sprite = sp;
    }

    public void Show(bool tLock) {
        StartCoroutine(FadeUpToFull(0.0f));
    	StartCoroutine(ExecuteAfterAnimating(() => {
            transparencyLock = tLock;
		}));
    }

    public void Hide() {
        transparencyLock = false;
        StartCoroutine(FadeDown(0.0f));
    }
}
