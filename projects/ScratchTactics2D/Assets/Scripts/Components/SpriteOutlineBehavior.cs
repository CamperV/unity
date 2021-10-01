using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[RequireComponent(typeof(Collider2D))]
public class SpriteOutlineBehavior : MonoBehaviour
{
    public static float standardThickness = 0.015f;
    public bool mouseOverEnabled = false;

	// set this only once, when one of MovingSprites is created
	public static Material staticMaterial;
    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    void Awake() {    
		if (SpriteOutlineBehavior.staticMaterial == null) {
			staticMaterial = Resources.Load<Material>("SpriteOutline");
		}
		material = staticMaterial;
        material.SetFloat("_Thickness", 0f);
    }

    void OnMouseEnter() {
        if (mouseOverEnabled) material.SetFloat("_Thickness", standardThickness);
    }

    void OnMouseExit() {
        if (mouseOverEnabled) material.SetFloat("_Thickness", 0f);
    }

    // used by external classes to set, i.e. Unit.OnSelect()
    public void SetOutline(bool on) {
        if (on) {
            material.SetFloat("_Thickness", standardThickness);
        } else {
            material.SetFloat("_Thickness", 0f);
        }
    }
}
