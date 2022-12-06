using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class MiniBarAnimator_UI : MonoBehaviour
{	
    public Color color_0;
    public Color color_1;

    private Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void InfiniFlash() => StartCoroutine( _Flash() );
    private IEnumerator _Flash() {
        float fixedTime = 0.5f;
		float timeRatio = 0.0f;

        while (true) {
            // to dim
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(color_0, color_1, timeRatio);
                yield return null;
            }

            // and then back up to normal color
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(color_1, color_0, timeRatio);
                yield return null;
            }
        }
    }
}
