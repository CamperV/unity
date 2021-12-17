using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class MiniHealthBar : MonoBehaviour
{	
    // this dict contains a float non-linear step to Lerp between for health::color relationships
    // ie, less than 5 is red, more than 15 is blue-green, etc
    // this is NOT a 0 - 100% scale, so that any full-health bar is the same color (even if one has 1 HP and one has 100 HP)
    // this would remain red even if at full-health, if around the threshold
    public Color color_0;
    public Color color_1;

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

    public Unit boundUnit;

	void Awake() {
        // get all your own members
        barLevel = GetComponentsInChildren<Transform>()[2];
        renderers = GetComponentsInChildren<SpriteRenderer>();

        backgroundRenderer = renderers[0];
        barRenderer        = renderers[1];
        borderRenderer     = renderers[2];

        // now, bind yourself to your parent Unit
        // just fail ungracefully if you don't have one, that shouldn't exist anyway
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        boundUnit.unitStats.UpdateHPEvent += UpdateBar;
    }

    public void UpdateBar(int val, int max) {
        currVal = val;
        maxVal = max;
        healthRatio = (float)currVal/(float)maxVal;
        Vector3 toScale = new Vector3(healthRatio, 1.0f, 1.0f);
        StartCoroutine(
            AnimateBar(barLevel.transform.localScale, toScale, Color.red, 1.0f, 1.0f)
        );

        barLevel.transform.localScale = toScale;
        barColor = HueSatLerp(color_0, color_1, healthRatio*healthRatio);
        barRenderer.color = barColor;

        // debug
        GetComponentInChildren<TextMeshPro>().SetText($"{currVal}/{maxVal}");
    }

    public static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }

    public IEnumerator AnimateBar(Vector3 fromScale, Vector3 toScale, Color color, float delayTime, float fixedTime) {
		GameObject go = new GameObject($"AnimBar", typeof(SpriteRenderer));

        SpriteRenderer goSR = go.GetComponent<SpriteRenderer>();
        goSR.sprite = barRenderer.sprite;
        goSR.color = color;
        goSR.sortingLayerName = barLevel.GetComponent<SpriteRenderer>().sortingLayerName;
        goSR.sortingOrder     = barLevel.GetComponent<SpriteRenderer>().sortingOrder - 1;

        go.transform.SetParent(barLevel.parent);
        go.transform.localPosition = barLevel.localPosition;
        go.transform.localScale = fromScale;


        // wait procedurally
        yield return new WaitForSeconds(delayTime);

        // ...then animate
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            go.transform.localScale = Vector3.Lerp(fromScale, toScale, timeRatio);
			yield return null;
		}

        Destroy(go);
    }
}
