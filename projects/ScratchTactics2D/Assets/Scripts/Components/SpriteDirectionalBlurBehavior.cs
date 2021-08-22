using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

public class SpriteDirectionalBlurBehavior : MonoBehaviour
{
    public static float maxBlurFactor = 0.0f;

	// set this only once, when one of MovingSprites is created
	public static Material staticMaterial;
    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    public float timeLagFactor = 0.05f;
    private float time = 0f;
    private Vector3 prevPosition;

    void Awake() {    
		if (SpriteDirectionalBlurBehavior.staticMaterial == null) {
			staticMaterial = Resources.Load<Material>("SpriteDirectionalBlur");
		}
		material = staticMaterial;
        material.SetVector("_Offset", new Vector2(0f, 0f));
        prevPosition = transform.position;  // init
    }

    void Update() {
        if (time > timeLagFactor) {
            Vector2 direction = (Vector2)(prevPosition - transform.position);
            float max = Mathf.Max( Mathf.Abs(direction.x), Mathf.Abs(direction.y) );
            float scaleFactor = maxBlurFactor / max;
            material.SetVector("_Offset", scaleFactor*direction);

            // and capture new prevPosition
            time = 0f;
            prevPosition = transform.position;
        }
        //
        time += Time.deltaTime;
    }
}
