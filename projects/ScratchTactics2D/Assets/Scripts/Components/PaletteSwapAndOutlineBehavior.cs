using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(BoxCollider2D))]
public class PaletteSwapAndOutlineBehavior : MonoBehaviour
{
    public static float standardThickness = 0.015f;
    public bool mouseOverEnabled = false;

    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    void Awake() {
        // default palette
        material.SetColor("_BrightColor", Color.cyan);
        material.SetColor("_MediumColor", Color.magenta);
        material.SetColor("_DarkColor", Color.blue);
        material.SetColor("_ShadowColor", Color.black);

        material.SetFloat("_OutlineThickness", 0f);
    }

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
