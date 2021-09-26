using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputListener : MonoBehaviour
{ 
    public KeyCode accelerator;
    public float movementAccel;
	public float phaseDelayTimeAccel;

	[HideInInspector] public Vector3 mouseWorldPos;

	public bool toggleMode = true;
	[HideInInspector] public bool toggle = false;

	void Update() {
		// get the collision point of the ray with the z = 0 plane
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		mouseWorldPos = ray.GetPoint(-ray.origin.z / ray.direction.z);

		// toggle the removal of movement animations
		if (toggleMode) {
			if (Input.GetKeyDown(accelerator)) {
				toggle = !toggle;

				SpriteAnimator.skipMovement = toggle;
				CameraManager.skipMovement = toggle;

				UIManager.inst.UpdateAccelerationToggleDisplay();
			}

		//else, hold-down-to-accelerate mode
		} else {
			if (Input.GetKeyDown(accelerator)) {
				SpriteAnimator.speedMultiplier *= movementAccel;

				UIManager.inst.UpdateAccelerationToggleDisplay(overrideBool: true);
			}
			if (Input.GetKeyUp(accelerator)) {
				SpriteAnimator.speedMultiplier /= movementAccel;

				UIManager.inst.UpdateAccelerationToggleDisplay(overrideBool: false);
			}
		}
    }
}
