using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmearShaderBehavior : MonoBehaviour
{
	// set this only once, when one of MovingSprites is created
	public static Material staticMaterial;
    public Material material {
        get => GetComponent<Renderer>().material;
        set => GetComponent<Renderer>().material = value;
    }

    public int frameLagFactor = 16;
    private Queue<Vector3> recentPositions = new Queue<Vector3>();

    void Awake() {    
		if (SmearShaderBehavior.staticMaterial == null) {
			staticMaterial = Resources.Load<Material>("SpriteSmear");
		}
		material = staticMaterial;
    }

    void LateUpdate() {
        if (recentPositions.Count > frameLagFactor) {
            material.SetVector("_SmearDirection", recentPositions.Dequeue());
        }

        material.SetVector("_Position", transform.position);
        recentPositions.Enqueue(transform.position);
    }
}