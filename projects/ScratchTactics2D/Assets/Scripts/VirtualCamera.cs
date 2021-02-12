using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera
{
    private bool draggingView;

    public VirtualCamera() {
        draggingView = false;
    }

    //
    // RMB drag view
    //
    public void DragUpdate(Battle battle) {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dragOffset = Vector3.zero;

        if (Input.GetMouseButtonDown(1) && !draggingView) {
            dragOffset = battle.transform.position - mouseWorldPos;
            draggingView = true;
        }
        // update pos by offset, release drag when mouse goes up
        if (draggingView) {
            battle.transform.position = mouseWorldPos + dragOffset;
        }

        // make sure we can drop out of the dragging mode
        if (draggingView && (!Input.GetMouseButton(1)))
            draggingView = false;
    }

    //
    // mouse view control
    //
    public void ScrollUpdate(Battle battle) {
        if (!draggingView) {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // calculate what the new scale WILL be
            // and calculate the scale ratio. Just use X, because our scale is uniform on all axes
            var updatedScale = battle.transform.localScale + (Input.GetAxis("Mouse ScrollWheel") * 0.75f) * Vector3.one;
            float scaleRatio = updatedScale.x / battle.transform.localScale.x;

            if (scaleRatio != 1.0f) {
                Vector3 localToMouse = battle.transform.position - mouseWorldPos;
                
                //update the scale, and position based on the new scale
                battle.transform.localScale = updatedScale;
                battle.transform.position = mouseWorldPos + (localToMouse * scaleRatio);
            }
        }
    }

    public void ZoomToAndLock(Battle battle, Vector3 target) {
        Vector3 screenPoint = Camera.main.transform.position;

        battle.transform.localScale = new Vector3(2f, 2f, 2f);
    }

    public void ReleaseLock(Battle battle) {
        battle.transform.localScale = Vector3.one;
        battle.transform.position = Camera.main.transform.position;
    }
}
