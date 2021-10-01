using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Renderer))]
public class PaletteSwapAndOutlineBehavior : MonoBehaviour
{
    public static float standardThickness = 0.01f;
    public bool mouseOverEnabled = false;

    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    // BoxCollider not REQUIRED, but if it exists:
    void OnMouseEnter() {
        if (mouseOverEnabled) material.SetFloat("_OutlineThickness", standardThickness);
    }

    void OnMouseExit() {
        if (mouseOverEnabled) material.SetFloat("_OutlineThickness", 0f);
    }

    // used by external classes to set, i.e. Unit.OnSelect()
    public void SetOutline(bool on) {
        if (on) {
            material.SetFloat("_OutlineThickness", standardThickness);
        } else {
            material.SetFloat("_OutlineThickness", 0f);
        }
    }

    // this assumes the correct color naming
    // palettes here have 100% alpha
    public void SetPalette(Dictionary<string, Color> palette) {
        foreach (string param in palette.Keys) {
            material.SetColor(param, palette[param]);
        }
    }
}
