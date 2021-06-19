using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public KeyCode accelerator;
    public float movementAccel;
	public float phaseDelayTimeAccel;
	
	private float _phaseDelayTime = 0.10f;

	void Start() {
		PhasedObject.phaseDelayTime = _phaseDelayTime;
	}

	void Update() {
		if (Input.GetKeyDown(accelerator)) {
			MovingObject.speedMultiplier *= movementAccel;
			PhasedObject.phaseDelayTime = phaseDelayTimeAccel;
		}
        if (Input.GetKeyUp(accelerator)) {
			MovingObject.speedMultiplier /= movementAccel;
			PhasedObject.phaseDelayTime = _phaseDelayTime;
		}
    }
}
