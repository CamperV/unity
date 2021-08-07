using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IMovable
{
	Transform safeTransform { get; }
	
	int movementStack { get; set; }
	bool isMoving { get; }
	void UpdateRealPosition(Vector3 pos);
}