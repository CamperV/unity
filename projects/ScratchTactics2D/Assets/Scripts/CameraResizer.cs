using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour
{
	public static void refitCamera(Vector3 pos, int height) {
		Camera.main.transform.position = pos;
		Camera.main.orthographicSize = (float)height / 2.0f;
	}
}