using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OverworldEnemySkeleton : OverworldEnemyBase
{	
	public override int detectionRange { get { return 3; } }
	public override HashSet<Type> untraversable {
		get {
			return new HashSet<Type>() { typeof(WaterWorldTile), typeof(MountainWorldTile) };
		}
	}
	
	// abstract implementations
	public override List<string> defaultUnitTags {
		get {
			return new List<string>() {
				"UnitSkeleton",
				"UnitSkeleton",
				"UnitSkeleton",
				//"UnitSkeleton",
				//"UnitSkeleton"
			};
		}
	}
}
