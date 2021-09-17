using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossBanditCampArmy : EnemyArmy
{	
	public override List<List<string>> spawnablePods {
		get => new List<List<string>>{
			new List<string>{
				"BerserkerClass", "BerserkerClass", "BerserkerClass", "BerserkerClass",
				// "ArcherClass", "ArcherClass", "ArcherClass", "ArcherClass", "ArcherClass",
				// "BanditClass", "BanditClass", "BanditClass", "BanditClass", "BanditClass", "BanditClass"
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
