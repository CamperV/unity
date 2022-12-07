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
    [SerializeField] private GameObject animBar;
    [SerializeField] private MiniBarAnimator_UI flashingBar;

    // void Awake() {
    //     GetComponent<UIAnchoredBobber>()?.SetPhase(10f * transform.GetSiblingIndex());
    // }

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
        animBar.gameObject.SetActive(true);
        animBar.transform.localScale = fromScale;

        // wait
        yield return new WaitForSeconds(delayTime);

        // ...then animate
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            animBar.transform.localScale = Vector3.Lerp(fromScale, toScale, timeRatio);
			yield return null;
		}

        animBar.gameObject.SetActive(false);
    }

    // visually flash the bar to demonstrate the health loss
    public void PreviewDamage(int damageAmountPreview) {
        flashingBar.gameObject.SetActive(true);
        flashingBar.transform.localScale = barLevel.transform.localScale;
        flashingBar.InfiniFlash();
        
        float previewHealthRatio = (float)Mathf.Max(0, currVal - damageAmountPreview)/(float)maxVal;
        ScaleBar(previewHealthRatio);
    }

    public void RevertPreview() {
        flashingBar.gameObject.SetActive(false);
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
        } else {
            barImage.color = color_0;
        }
    }
}
