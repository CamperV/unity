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
        UpdatePoiseEvent,
        UpdateDamageReductionEvent
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
    [SerializeField] private MiniBarAnimator_UI flashingDamageRangeBar;

    public void AttachTo(Unit thisUnit) {
        switch (registerTo) {
            case RegistrationOptions.UpdateHPEvent:
                UpdateBar(thisUnit.statSystem.MAX_HP, thisUnit.statSystem.MAX_HP);
                thisUnit.statSystem.UpdateHPEvent += UpdateBar;
                break;

            case RegistrationOptions.UpdatePoiseEvent:
                UpdateBar(thisUnit.statSystem.MAX_POISE, thisUnit.statSystem.MAX_POISE);
                thisUnit.statSystem.UpdatePoiseEvent += UpdateBar;
                break;

            case RegistrationOptions.UpdateDamageReductionEvent:
                UpdateBar(thisUnit.statSystem.DAMAGE_REDUCTION, thisUnit.statSystem.DAMAGE_REDUCTION);
                thisUnit.statSystem.UpdateDamageReductionEvent += d => UpdateBar(d, 0);
                break;

            default:
                break;
        }
    }

    // visually flash the bar to demonstrate the health loss
    public void PreviewDamage(int damageAmountPreview) {
        // flash the full value of health loss
        flashingBar.gameObject.SetActive(true);
        flashingBar.transform.localScale = barLevel.transform.localScale;
        flashingBar.InfiniFlash();
        
        // update bar value to scale with the preview, and "reveal" the flashing underneath
        float previewHealthRatio = (float)Mathf.Max(0, currVal - damageAmountPreview)/(float)maxVal;
        ScaleBar(previewHealthRatio);
    }

    // visually flash the bar to demonstrate the health loss
    public void PreviewDamage(int minDamage, int maxDamage) =>_PreviewDamage(minDamage, maxDamage);
    public void PreviewDamage(Damage damage) => _PreviewDamage(damage.min, damage.max);
    private void _PreviewDamage(int minDamage, int maxDamage) {
        // flash the full value of health loss
        flashingBar.gameObject.SetActive(true);
        flashingBar.transform.localScale = barLevel.transform.localScale;
        flashingBar.InfiniFlash();

        // flash the loss if "min" damage
        flashingDamageRangeBar.gameObject.SetActive(true);
        float minRatio = (float)Mathf.Max(0, currVal - minDamage)/(float)maxVal;
        flashingDamageRangeBar.transform.localScale = new Vector3(minRatio, 1f, 1f);
        // flashingDamageRangeBar.InfiniFlash();
        
        // update bar value to scale with the preview, and "reveal" the flashing underneath
        float maxRatio = (float)Mathf.Max(0, currVal - maxDamage)/(float)maxVal;
        ScaleBar(maxRatio);
    }

    public void RevertPreview() {
        flashingBar.gameObject.SetActive(false);
        flashingDamageRangeBar.gameObject.SetActive(false);
        ScaleBar(ratio);
    }

    private void UpdateBar(int val, int max) {
        currVal = val;
        maxVal = max;
        ratio = (float)currVal/(float)maxVal;
        Vector3 toScale = new Vector3(ratio, 1.0f, 1.0f);

        StopAllCoroutines();
        StartCoroutine(
            Utils.QueueCoroutines(
                _UpdateBarVisual(ratio),
                AnimateBar(barLevel.transform.localScale, toScale, Color.white, 1.0f, 1.0f)
            )
        );
    }

    private IEnumerator AnimateBar(Vector3 fromScale, Vector3 toScale, Color color, float delayTime, float fixedTime) {
        animBar.gameObject.SetActive(true);
        // animBar.transform.localScale = fromScale;
        Vector3 originalScale = animBar.transform.localScale;

        // wait
        yield return new WaitForSeconds(delayTime);

        // ...then animate
		float timeRatio = 0.0f;
		
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            animBar.transform.localScale = Vector3.Lerp(originalScale, toScale, timeRatio);
			yield return null;
		}

        animBar.gameObject.SetActive(false);
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

    private static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }
}
