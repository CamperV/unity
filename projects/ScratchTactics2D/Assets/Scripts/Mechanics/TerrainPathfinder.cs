using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ElevationPathfinder : Pathfinder
{
	public ElevationPathfinder(ElevationMap map) {
		pathableSurface = map;
	}
}
