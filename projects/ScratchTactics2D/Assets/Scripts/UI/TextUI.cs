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

        Debug.Log($"My mesh renderer is on layer: {meshRenderer.sortingLayerName}");
        textMesh.color = Color.red;
    }

    public override void UpdateTransparency(float alpha) {
        textMesh.color = textMesh.color.WithAlpha(alpha);
    }

    public void SetText(string message) {
        textMesh.SetText(message);
    }
}
