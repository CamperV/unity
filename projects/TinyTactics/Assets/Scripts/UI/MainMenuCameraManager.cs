using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainMenuCameraManager : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
	private Vector3 anchorPosition;

	void Start() {
		anchorPosition = transform.position;
	}

    public void SetAnchorTransform(RectTransform rt) => SetAnchor(rt.position);
    public void SetAnchor(Vector3 newAnchor) => anchorPosition = new Vector3(newAnchor.x, newAnchor.y, transform.position.z);

	// move the tracking position based on movement and clamp it into bounds
	public void Update() {
		transform.position = Vector3.Lerp(transform.position, anchorPosition, Time.deltaTime*cameraSpeed);
	}
}