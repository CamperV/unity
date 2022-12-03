using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class MiniBar_UI : MonoBehaviour
{	
    public enum RegistrationOptions {
        UpdateHPEvent,
        UpdateBreakEvent
    };
    [SerializeField] private RegistrationOptions registerTo;

    [SerializeField] private bool useRatioColor;

    public Color color_0;
    public Color color_1;

    [HideInInspector] public int currVal;
    [HideInInspector] public int maxVal;
    [HideInInspector] public float ratio;

    [SerializeField] private Transform barLevel;
    [HideInInspector] public Color barColor;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image barImage;
    [SerializeField] private Image borderImage;

    public void AttachTo(Unit thisUnit) {
        switch (registerTo) {
            case RegistrationOptions.UpdateHPEvent:
                UpdateBar(thisUnit.unitStats.VITALITY, thisUnit.unitStats.VITALITY);
                thisUnit.unitStats.UpdateHPEvent += UpdateBar;
                break;

            case RegistrationOptions.UpdateBreakEvent:
                UpdateBar(thisUnit.unitStats.BRAWN, thisUnit.unitStats.BRAWN);
                thisUnit.unitStats.UpdateBreakEvent += UpdateBar;
                break;

            default:
                break;
        }
    }

    private void UpdateBar(int val, int max) {
        currVal = val;
        maxVal = max;
        ratio = (float)currVal/(float)maxVal;
        Vector3 toScale = new Vector3(ratio, 1.0f, 1.0f);

        StartCoroutine(
            Utils.QueueCoroutines(
                _UpdateBarVisual(ratio),
                AnimateBar(barLevel.transform.localScale, toScale, Color.white, 1.0f, 1.0f)
            )
        );
    }

    public IEnumerator AnimateBar(Vector3 fromScale, Vector3 toScale, Color color, float delayTime, float fixedTime) {
		GameObject go = new GameObject($"AnimBar", typeof(Image));

        Image goSR = go.GetComponent<Image>();
        goSR.sprite = barImage.sprite;
        goSR.color = color;

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
    private MiniBarAnimator_UI flashingBar;
    [SerializeField] private MiniBarAnimator_UI flashingBarPrefab;

    public void PreviewDamage(int damageAmountPreview) {
        float previewHealthRatio = (float)Mathf.Max(0, currVal - damageAmountPreview)/(float)maxVal;

        Image levelImage = barLevel.GetComponent<Image>();
        flashingBar = Instantiate(flashingBarPrefab, barLevel.parent);
        flashingBar.Reposition(barLevel.localPosition, barLevel.localScale);
        flashingBar.InfiniFlash();

        ScaleBar(previewHealthRatio);
    }

    public void RevertPreview() {
        Destroy(flashingBar.gameObject);
        ScaleBar(ratio);
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
        if (useRatioColor) {
            barColor = HueSatLerp(color_0, color_1, ratio*ratio);
            barImage.color = barColor;
        }
    }
}
