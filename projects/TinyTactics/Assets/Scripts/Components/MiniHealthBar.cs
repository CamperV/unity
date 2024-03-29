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

    [SerializeField] private Transform barLevel;
    public Color barColor;

    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer barRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;

    private Unit boundUnit;
    [SerializeField] private TextMeshPro textValue;

    void Awake() {
        boundUnit = GetComponentInParent<Unit>();
        Debug.Assert(boundUnit != null);
    }

    void Start() {
        UpdateBar(boundUnit.statSystem.MAX_HP, boundUnit.statSystem.MAX_HP);
        boundUnit.statSystem.UpdateHPEvent += UpdateBar;
    }

    private void SetAlpha(float alpha) {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            sr.color = sr.color.WithAlpha(alpha);
        }
    }

    public void UpdateBar(int val, int max) {
        bool healing = val > currVal;

        currVal = val;
        maxVal = max;
        healthRatio = (float)currVal/(float)maxVal;
        Vector3 toScale = new Vector3(healthRatio, 1.0f, 1.0f);

        StartCoroutine(
            Utils.QueueCoroutines(
                _UpdateBarVisual(healthRatio),
                AnimateBar(barLevel.transform.localScale, toScale, Color.red, 1.0f, 1.0f)
            )
        );

        textValue.SetText($"{currVal}/{maxVal}");
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

        // wait
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

    // visually flash the bar to demonstrate the health loss
    private MiniBarAnimator flashingBar;
    [SerializeField] private MiniBarAnimator flashingBarPrefab;

    public void PreviewDamage(int damageAmountPreview) {
        float previewHealthRatio = (float)Mathf.Max(0, currVal - damageAmountPreview)/(float)maxVal;

        SpriteRenderer levelSR = barLevel.GetComponent<SpriteRenderer>();
        flashingBar = Instantiate(flashingBarPrefab, barLevel.parent);
        flashingBar.Reposition(barLevel.localPosition, barLevel.localScale, levelSR.sortingLayerName, levelSR.sortingOrder - 1);
        flashingBar.InfiniFlash();

        ScaleBar(previewHealthRatio);
    }

    public void RevertPreview() {
        Destroy(flashingBar.gameObject);
        ScaleBar(healthRatio);
    }

    private static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }

    private IEnumerator _UpdateBarVisual(float ratio) {
        ScaleBar(ratio);
        yield break;
    }

    private void ScaleBar(float ratio) {
        Vector3 toScale = new Vector3(ratio, 1f, 1f);
        barLevel.transform.localScale = toScale;
        barColor = HueSatLerp(color_0, color_1, ratio*ratio);
        barRenderer.color = barColor;
    }
}
