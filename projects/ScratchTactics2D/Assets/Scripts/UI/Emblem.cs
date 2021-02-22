using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class Emblem : UnitUIElement
{	
    private SpriteRenderer spriteRenderer;
	private float spriteHeight { get => spriteRenderer.bounds.size.y; }

	void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set renderer properties
		spriteRenderer.sortingLayerName = "Tactics UI";
		spriteRenderer.sortingOrder = 0;

        //
        transform.localScale = 0.15f * Vector3.one;
    }

    void Start() {
        // scale based on healthBar
        transform.position -= new Vector3(parentUI.healthBar.spriteWidth * 0.5f * 1.05f, parentUI.healthBar.spriteHeight * 1.75f, 0);
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
