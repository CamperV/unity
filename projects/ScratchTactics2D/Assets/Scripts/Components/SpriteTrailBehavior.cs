using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteTrailBehavior : MonoBehaviour
{
    // enable this is you want to attach a shader
    //
	// set this only once
	// public static Material staticMaterial;
    // public Material material {
    //     get => GetComponent<Renderer>().material;
    //     set => GetComponent<Renderer>().material = value;
    // }
    //
    // void Awake() {    
	// 	if (SmearShaderBehavior.staticMaterial == null) {
	// 		staticMaterial = Resources.Load<Material>("SpriteSmear");
	// 	}
	// 	material = staticMaterial;
    // }
    public float fadeTime = 0.25f;
    public float iterTime = 0.05f;
    private float timeCounter = 0f;

    void Update() {
        timeCounter += Time.deltaTime;

        if (timeCounter > iterTime) {
            timeCounter = 0f;

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;

            // clone a new GameObject with no activity on it
            GameObject spriteReplica = new GameObject($"SpriteTrailBehavior_{sprite.name}");
            spriteReplica.AddComponent<SpriteRenderer>();
            spriteReplica.AddComponent<SpriteAnimator>();

            spriteReplica.transform.position = transform.position;
            spriteReplica.GetComponent<SpriteRenderer>().sprite = sprite;
            spriteReplica.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
            spriteReplica.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;

            // With URP enabled, the default material is Sprite-Lit-Default
            // since we have no illumination in this project, gotta find the old one...
            spriteReplica.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));

            spriteReplica.GetComponent<MonoBehaviour>().StartCoroutine(
                spriteReplica.GetComponent<SpriteAnimator>().FadeDownThenDestroy(fadeTime)
            );
        }

    }
}