using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OverworldEnemySkeleton : OverworldEnemyBase
{	
	public override int detectionRange { get { return 2; } }
	public override HashSet<Type> spawnable {
		get {
			return new HashSet<Type>() {
				typeof(ForestWorldTile),
				typeof(ForestRoadWorldTile),
				typeof(MountainRoadWorldTile),
			};
		}
	}
	
	// abstract implementations
	public override List<string> defaultUnitTags {
		get {
			return new List<string>() {
				"UnitSkeleton",
				"UnitSkeleton"
			};
		}
	}
}
