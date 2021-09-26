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

	void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set renderer properties
		spriteRenderer.sortingLayerName = "Tactics UI";
		spriteRenderer.sortingOrder = 0;
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
