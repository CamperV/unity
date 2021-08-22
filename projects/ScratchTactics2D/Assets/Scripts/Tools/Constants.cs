using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global constants (const is already static)
public class Constants {
	public const int standardTickCost = 100;

	public static readonly Color selectColorWhite  = new Color(1.00f, 1.00f, 1.00f, 0.45f);
	public static readonly Color selectColorBlue   = new Color(0.00f, 0.30f, 0.75f, 0.75f);
	public static readonly Color threatColorRed    = new Color(0.75f, 0.20f, 0.20f, 0.75f);
	public static readonly Color threatColorYellow = new Color(0.75f, 0.75f, 0.00f, 0.75f);
	public static readonly Color threatColorIndigo = new Color(0.30f, 0.30f, 0.75f, 0.75f);
	public static readonly Color zoneColorViolet   = new Color(0.75f, 0.30f, 0.75f, 0.75f);
	public static readonly Color hideColorBlack    = new Color(0.15f, 0.15f, 0.15f, 1.00f);

	public static readonly Vector3Int unselectableVector3Int = int.MinValue * Vector3Int.one;

	// relates to the y/z sorting priority, see Project Settings -> Custom Sort Axis
	public static readonly int zSortingConstant = 2;
}