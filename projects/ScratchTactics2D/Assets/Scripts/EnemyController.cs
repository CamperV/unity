using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPhasedObject
{
	private List<Enemy> enemyList;
	
	public bool phaseActionTaken { get; set; }
	
	void Awake() {
		phaseActionTaken = false;
		enemyList = new List<Enemy>();
    }

    void Update() {
        if (!MyPhase()) return;
		
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		bool pAT = true;
		foreach (Enemy enemy in enemyList) {
			pAT &= enemy.TakePhaseAction();
		}
		
		return pAT;
	}
	
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
}
