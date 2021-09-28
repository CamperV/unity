using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;

public class MiniHealthBar : UnitUIElement
{	
    // this dict contains a float non-linear step to Lerp between for health::color relationships
    // ie, less than 5 is red, more than 15 is blue-green, etc
    // this is NOT a 0 - 100% scale, so that any full-health bar is the same color (even if one has 1 HP and one has 100 HP)
    // this would remain red even if at full-health, if around the threshold
    public Color color_0;
    public Color color_1;
    // public Color color_2;
    // public Color color_3;
    // public Color color_4;
    // public Color color_5;
    public Dictionary<int, Color> colorLevels;

    public int currVal;
    public int maxVal;
    public float healthRatio;

    [HideInInspector] public Transform barLevel;
    public Color barColor;

    [HideInInspector] public SpriteRenderer[] renderers;
    SpriteRenderer backgroundRenderer;
    SpriteRenderer barRenderer;
    SpriteRenderer borderRenderer;
	public float spriteWidth { get => backgroundRenderer.size.x; }
	public float spriteHeight { get => backgroundRenderer.size.y; }

	void Awake() {
        barLevel = GetComponentsInChildren<Transform>()[2];
        renderers = GetComponentsInChildren<SpriteRenderer>();

        backgroundRenderer = renderers[0];
        barRenderer        = renderers[1];
        borderRenderer     = renderers[2];

        colorLevels = new Dictionary<int, Color>{
            [ 0] = color_0,
            [ 5] = color_1,
            // [10] = color_2,
            // [15] = color_3,
            // [25] = color_4,
            // [40] = color_5
        };
    }

    void Start() {
        transparencyLock = true;
    }

    public void UpdateBar(int val, int max, float alpha) {
        currVal = val;
        maxVal = max;
        healthRatio = (float)currVal/(float)maxVal;
        barLevel.transform.localScale = new Vector3(healthRatio, 1.0f, 1.0f);

        barColor = HueSatLerp(color_0, color_1, healthRatio*healthRatio);
        barRenderer.color = barColor;
        UpdateTransparency(alpha);
    }

    public static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }

    public override void UpdateTransparency(float alpha) {
        if (transparencyLock) return;

        foreach (SpriteRenderer rend in renderers) {
            rend.color = rend.color.WithAlpha(alpha);
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
