using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

public class SpriteDirectionalBlurBehavior : MonoBehaviour
{
	// set this only once, when one of MovingSprites is created
	public static Material staticMaterial;
    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    public float timeLagFactor = 0.5f;
    private float time = 0f;
    private Vector3 prevPosition;

    void Awake() {    
		if (SpriteDirectionalBlurBehavior.staticMaterial == null) {
			staticMaterial = Resources.Load<Material>("SpriteDirectionalBlur");
		}
		material = staticMaterial;
        material.SetVector("_Offset", new Vector2(0.03f, 0.03f));
        prevPosition = transform.position;  // init
    }

    // void Update() {
    //     if (time > timeLagFactor) {
    //         //Vector2 direction = 0.02f * Vector3.Normalize((transform.position - prevPosition)).Trim("z");
    //         material.SetVector("_Offset", direction);

    //         // and capture new prevPosition
    //         time = 0f;
    //         prevPosition = transform.position;
    //     }
    //     //
    //     time += Time.deltaTime;
    // }
}
