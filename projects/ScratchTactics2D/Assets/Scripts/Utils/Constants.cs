using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global constants (const is already static)
public class Constants {
	public const int standardTickCost = 100;
/*
	public const Color selectColorWhite  = new Color(1.00f, 1.00f, 1.00f, 0.45f);
	public const Color selectColorBlue   = new Color(0.00f, 0.30f, 0.75f, 0.75f);
	public const Color threatColorRed    = new Color(0.75f, 0.20f, 0.20f, 0.75f);
	public const Color threatColorYellow = new Color(0.75f, 0.75f, 0.00f, 0.75f);
	public const Color threatColorIndigo = new Color(0.30f, 0.30f, 0.75f, 0.75f);*/
	public static readonly Color selectColorWhite  = new Color(1.00f, 1.00f, 1.00f, 1f);
	public static readonly Color selectColorBlue   = new Color(0.00f, 0.30f, 0.75f, 1f);
	public static readonly Color threatColorRed    = new Color(0.75f, 0.20f, 0.20f, 1f);
	public static readonly Color threatColorYellow = new Color(0.75f, 0.75f, 0.00f, 1f);
	public static readonly Color threatColorIndigo = new Color(0.30f, 0.30f, 0.75f, 1f);
}