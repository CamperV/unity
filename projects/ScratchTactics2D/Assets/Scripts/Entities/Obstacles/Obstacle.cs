using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public class Obstacle : TacticsEntityBase
{
	private readonly float spriteScaleFactor = 0.80f;

	public override float zHeight { get => 2; }

	void Start() {
        transform.localScale = new Vector3(spriteScaleFactor*spriteScaleFactor, spriteScaleFactor, 1.0f);
	}
}
