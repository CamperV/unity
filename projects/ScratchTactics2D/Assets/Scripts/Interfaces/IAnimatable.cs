using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IAnimatable
{
	Transform safeTransform { get; }
	SpriteRenderer spriteRenderer { get; }
	
	int animationStack { get; set; }
	bool isAnimating { get; }
}