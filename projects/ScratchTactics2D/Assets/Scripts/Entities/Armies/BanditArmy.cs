using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BanditArmy : EnemyArmy
{	
	public override int detectionRange { get { return 2; } }
	
	// abstract implementations
	public override List<string> defaultUnitTags {
		get {
			return new List<string>() {
				"UnitSkeleton",
				"UnitSkeleton"
			};
		}
	}

	public override void OnHit() {
		return;
	}
	
	public override void OnAlert() {
		animator.SetTrigger("SkeletonAlert");
	}
}
