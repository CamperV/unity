using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    public KeyCode accelerator;
    public float movementAccel;
    public float phaseAccel;

	void Update() {
		if (Input.GetKeyDown(accelerator)) {
			MovingObject.speedMultiplier *= movementAccel;
			PhasedObject.phaseDelayTime /= phaseAccel;
		}
        if (Input.GetKeyUp(accelerator)) {
			MovingObject.speedMultiplier /= movementAccel;
			PhasedObject.phaseDelayTime *= phaseAccel;
		}
    }
}
