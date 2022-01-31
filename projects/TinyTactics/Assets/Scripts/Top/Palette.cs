using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global constants (const is already static)
public struct Palette {
	public static readonly Color selectColorWhite  = new Color(1.00f, 1.00f, 1.00f, 0.45f);
	public static readonly Color selectColorBlue   = new Color(0.00f, 0.30f, 0.75f, 0.75f);

	public static readonly Color reservedColorBlue = new Color(0.00f, 0.30f, 0.75f, 0.25f);
	
	public static readonly Color threatColorRed    = new Color(0.75f, 0.20f, 0.20f, 0.75f);
	public static readonly Color threatColorPink   = new Color(0.75f, 0.40f, 0.40f, 0.75f);
	public static readonly Color threatColorYellow = new Color(0.65f, 0.65f, 0.15f, 0.75f);
	public static readonly Color threatColorIndigo = new Color(0.30f, 0.30f, 0.75f, 0.75f);
	public static readonly Color threatColorViolet = new Color(0.75f, 0.30f, 0.75f, 0.75f);

	public static readonly Color healColorGreen    = new Color(0.00f, 0.75f, 0.30f, 0.75f);
	public static readonly Color hideColorBlack    = new Color(0.15f, 0.15f, 0.15f, 1.00f);

	public static readonly Color normalRed 		   = new Color(0.75f, 0.20f, 0.20f, 1.00f);
	public static readonly Color enemyRedPinkColor = new Color(1.00f, 0.41f, 0.59f, 1.00f);
}