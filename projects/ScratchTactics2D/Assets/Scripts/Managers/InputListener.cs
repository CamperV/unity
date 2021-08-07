using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public KeyCode accelerator;
    public float movementAccel;
	public float phaseDelayTimeAccel;
	
	private float _phaseDelayTime = 0.00f;

	void Start() {
		PhasedObject.phaseDelayTime = _phaseDelayTime;
	}

	void Update() {
		if (Input.GetKeyDown(accelerator)) {
			SpriteAnimator.speedMultiplier *= movementAccel;
			PhasedObject.phaseDelayTime = phaseDelayTimeAccel;
		}
        if (Input.GetKeyUp(accelerator)) {
			SpriteAnimator.speedMultiplier /= movementAccel;
			PhasedObject.phaseDelayTime = _phaseDelayTime;
		}
    }
}
