using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public KeyCode accelerator;
    public float movementAccel;
	public float phaseDelayTimeAccel;

	[HideInInspector] public Vector3 mouseWorldPos;
	
	// private float _phaseDelayTime = 0.00f;

	void Start() {
		// PhasedObject.phaseDelayTime = _phaseDelayTime;
	}

	void Update() {
		// get the collision point of the ray with the z = 0 plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mouseWorldPos = ray.GetPoint(-ray.origin.z / ray.direction.z);

		if (Input.GetKeyDown(accelerator)) {
			SpriteAnimator.speedMultiplier *= movementAccel;
			// PhasedObject.phaseDelayTime = phaseDelayTimeAccel;
		}
        if (Input.GetKeyUp(accelerator)) {
			SpriteAnimator.speedMultiplier /= movementAccel;
			// PhasedObject.phaseDelayTime = _phaseDelayTime;
		}
    }
}
