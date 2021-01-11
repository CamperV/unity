using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

public class TextUI : UnitUIElement
{
    private TextMeshPro textMesh;
    public MeshRenderer meshRenderer;

    void Awake() {
        textMesh = GetComponent<TextMeshPro>();
        meshRenderer = GetComponent<MeshRenderer>();

    	// set meshrenderer properties
		meshRenderer.sortingLayerName = "Tactics UI";
		meshRenderer.sortingOrder = 0;
    }

    public override void UpdateTransparency(float alpha) {
        textMesh.color = textMesh.color.WithAlpha(alpha);
    }

    public void SetText(string message) {
        textMesh.SetText(message);
    }


    public void Bold() {
        textMesh.fontStyle = FontStyles.Bold;
    }

    public void SetColor(Color c) {
        textMesh.color = c;
    }

    public void SetScale(float scale) {
        textMesh.transform.localScale *= scale;
    }

    // float away over a certain time, with the distance being a function of our Mesh's size
    public IEnumerator FloatAway(float fixedTime, float fixedDistance) {
        animFlag = true;

		float timeRatio = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Vector3 endPos = startPos + new Vector3(0, fixedDistance, 0);

        // sine values
        float amplitude = fixedDistance / 2.0f;
        float freq = 4.0f;

        // calculate to offset the starting position of the sine
        float phase = -1*Mathf.Sin(freq*Time.time);

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            
            // move upwards, oscillate, shrink, fade, and eventually terminate self
			transform.position = Vector3.Lerp(startPos, endPos, timeRatio);
            transform.position += new Vector3(amplitude * Mathf.Sin(freq*Time.time + phase), 0, 0);
            transform.localScale = startScale * (1.0f - (timeRatio / 4.0f));
            UpdateTransparency(1.0f - timeRatio);
			yield return null;
		}

        animFlag = false;
        Destroy(gameObject);
	}
}
