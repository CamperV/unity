using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera
{
    public Battle battle;

    private Vector3 dragOffset;
    private bool draggingView;
    //
    private Vector3 lockedPosition;
    private Vector3 lockedScale;
    private bool viewLock;

    public float currentZoomLevel { get => battle.transform.localScale.x; }

    public VirtualCamera(Battle battleToRegister) {
        battle = battleToRegister;

        dragOffset = Vector3.zero;
        draggingView = false;
        //
        lockedPosition = Vector3.zero;
        lockedScale = Vector3.one;
        viewLock = false;
    }

    //
    // RMB drag view
    //
    public void DragUpdate() {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1) && !draggingView) {
            dragOffset = battle.transform.position - mouseWorldPos;
            draggingView = true;
        }
        // update pos by offset, release drag when mouse goes up
        if (draggingView) {
            battle.transform.position = mouseWorldPos + dragOffset;
        }

        // make sure we can drop out of the dragging mode
        if (draggingView && !Input.GetMouseButton(1)) {
            draggingView = false;
        }
    }

    //
    // mouse view control
    //
    public void ScrollUpdate() {
        if (!draggingView) {
            // calculate what the new scale WILL be
            // and calculate the scale ratio. Just use X, because our scale is uniform on all axes
            var updatedScale = battle.transform.localScale + (Input.GetAxis("Mouse ScrollWheel") * 0.75f) * Vector3.one;
            float scaleRatio = updatedScale.x / battle.transform.localScale.x;

            if (scaleRatio != 1.0f) {
                Vector3 localToMouse = battle.transform.position - GameManager.inst.mouseManager.mouseWorldPos;
                
                //update the scale, and position based on the new scale
                battle.transform.localScale = updatedScale;
                battle.transform.position = GameManager.inst.mouseManager.mouseWorldPos + (localToMouse * scaleRatio);
            }
        }
    }

    public void Zoom(float zoomLevel) {
        Vector3 updatedScale = zoomLevel * Vector3.one;
        battle.StartCoroutine( SmoothCameraMovement(0.15f, battle.transform.position, updatedScale) );
    }

    public void ZoomToAndLock(Vector3 target, float zoomLevel) {
        lockedPosition = battle.transform.position;
        lockedScale = battle.transform.localScale;
        viewLock = true;

        Vector3 screenPoint = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
        Vector3 updatedScale = zoomLevel * Vector3.one;
        float scaleRatio = updatedScale.x / lockedScale.x;
        
        // move the selected target position to Camera.main.x/y position
        Vector3 toPosition = screenPoint - (target - battle.transform.position)*scaleRatio;
        battle.StartCoroutine( SmoothCameraMovement(0.15f, toPosition, updatedScale) );
    }

    public void ReleaseLock() {
        if (viewLock) {
            battle.StartCoroutine( SmoothCameraMovement(0.15f, lockedPosition, lockedScale) );
            lockedPosition = battle.transform.position;
            lockedScale = battle.transform.localScale;
            viewLock = false;
        }
    }

    private IEnumerator SmoothCameraMovement(float fixedTime, Vector3 toPosition, Vector3 toScale) {
		float timeRatio = 0.0f;
        Vector3 startPos = battle.transform.position;
        Vector3 startScale = battle.transform.localScale;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
			battle.transform.position = Vector3.Lerp(startPos, toPosition, timeRatio);
            battle.transform.localScale = Vector3.Lerp(startScale, toScale, timeRatio);
			yield return null;
		}
		
		// after the while loop is broken:
		battle.transform.position = toPosition;
        battle.transform.localScale = toScale;
    }
}
