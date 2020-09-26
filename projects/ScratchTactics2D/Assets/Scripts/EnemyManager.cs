using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : PhasedObject, IPhasedObject
{
	private List<Enemy> enemyList;
	
	private bool subjectsActingTrigger;

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;

	void Awake() {
		myPhase = Enum.Phase.enemy;
		subjectsActingTrigger = false;
		
		enemyList = new List<Enemy>();
    }

    void Update() {
        if (!MyPhaseActive()) return;
		//
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				TakePhaseAction();
				break;
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				break;
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase();
				break;
			case Enum.PhaseActionState.postPhaseDelay:
				// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhase:
				break;
		}
    }
	
	// overrides base
	public void TriggerPhase() {
		base.TriggerPhase();
		subjectsActingTrigger = true;
	}
	
	public override void TakePhaseAction() {	
		// start action coroutine if not currently running
		// reset trigger immediately
		if (subjectsActingTrigger) {
			subjectsActingTrigger = false;
			phaseActionState = Enum.PhaseActionState.acting;
			
			// update player position
			playerPosLastTurn = lastKnownPlayerPos;
			lastKnownPlayerPos = GameManager.inst.player.gridPosition;

			StartCoroutine(SubjectTakePhaseActions());
		}
	}
	
	public IEnumerator SubjectTakePhaseActions() {
		for (int i = 0; i < enemyList.Count; i++) {		
			switch(enemyList[i].state) {
				case Enum.EnemyState.idle:
					if (enemyList[i].InLineOfSight(lastKnownPlayerPos)) {
						enemyList[i].state = Enum.EnemyState.tracking;
					} else {
						enemyList[i].TakeIdleAction();
					}
					break;
				case Enum.EnemyState.tracking:
					enemyList[i].MoveTowardsPosition(lastKnownPlayerPos);
					break;
			}
			
			// don't delay if you're the last
			if (i == enemyList.Count-1) {
				yield return null;
			} else {
				yield return new WaitForSeconds(phaseDelayTime);
			}
		}
		
		phaseActionState = Enum.PhaseActionState.complete;
	}
		
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
		
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
}
