using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BerserkerArmy : EnemyArmy
{	
	public override int detectionRange { get { return 2; } }

	public override List<List<string>> spawnablePods {
		get => new List<List<string>>{
			new List<string>{
				"BerserkerClass", "BerserkerClass"
			},
			new List<string>{
				"BerserkerClass",
				"ArcherClass"
			},
			new List<string>{
				"BerserkerClass",
				"BanditClass", "BanditClass"
			}
		};
	}

	public override void OnHit() {
		return;
	}
	
	public override void OnAlert() {
		//animator.SetTrigger("SkeletonAlert");
	}
}
