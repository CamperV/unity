using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Extensions;

public class MiniHealthBar : UnitUIElement
{	
    private int _currVal;
    [HideInInspector] public int currVal {
        get => _currVal;
        set {
            _currVal = Mathf.Max(value, 0);
        }
    }
    [HideInInspector] public int maxPips = 1;
    [HideInInspector] public float healthRatio;
    [HideInInspector] public Transform barLevel;

    [HideInInspector] public SpriteRenderer[] renderers;
	public float spriteWidth { get => renderers[1].bounds.size.x; }
	public float spriteHeight { get => renderers[1].bounds.size.y; }

	void Awake() {
        // I don't like how these are implemented, but c'est la Unity
        // this vvv is the first CHILD Transform, b/c GetCom... gets the parent too
        barLevel = GetComponentsInChildren<Transform>()[1];
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start() {
        transform.localScale = new Vector3(0.45f, 0.75f, 1.0f);
        transform.position -= new Vector3(0, spriteHeight*1.75f, 0);
    }

    public void InitHealthBar(int m) {
        maxPips = m;
        UpdateBar(maxPips, 1.0f);
        
        transparencyLock = true;
    }

    public void UpdateBar(int val, float alpha) {
        Debug.Log($"From {currVal} to {val}");
        float scaleRatio = (float)currVal/(float)val;
        currVal = val;
        healthRatio = (float)currVal/(float)maxPips;

        barLevel.transform.localScale = new Vector3(0.01f + healthRatio, 1.0f, 1.0f);
        Debug.Log($"Updated ratio: {healthRatio} ({barLevel.transform.localScale})");

        //barLevel.transform.position -= new Vector3(spriteWidth * scaleRatio, 0, 0);

        UpdateTransparency(alpha);
    }

    public override void UpdateTransparency(float alpha) {
        if (transparencyLock) return;

        foreach (SpriteRenderer sr in renderers) {
            sr.color = sr.color.WithAlpha(alpha);
        }
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
