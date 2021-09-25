using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BanditArmy : EnemyArmy
{	
	public override int detectionRange { get { return 3; } }

	public override List<List<string>> spawnablePods {
		get => new List<List<string>>{
			new List<string>{
				"BanditClass", "BanditClass",
				"ArcherClass"
			},
			new List<string>{
				"BanditClass",
				"ArcherClass", "ArcherClass"
			},
			new List<string>{
				"BanditClass", "BanditClass", "BanditClass"
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
