using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IPhasedObject
{
	private List<Enemy> enemyList;
	private HashSet<Vector3Int> _pathTiles;
	
	private bool subjectsActingTrigger;
	private bool allSubjectsPhaseActionTaken;
	
	[HideInInspector] public bool phaseActionTaken { get; private set; }

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;

	
	void Awake() {
		phaseActionTaken = false;
		subjectsActingTrigger = false;
		allSubjectsPhaseActionTaken = false;
		
		enemyList = new List<Enemy>();
		_pathTiles = new HashSet<Vector3Int>();
    }

    void Update() {
        if (!MyPhase()) return;
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.phaseManager.currentPhase == PhaseManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public void TriggerPhase() {
		phaseActionTaken = false;
		subjectsActingTrigger = true;
	}
	
	public bool TakePhaseAction() {	
		// start action coroutine if not currently running
		// reset trigger immediately
		if (subjectsActingTrigger) {
			subjectsActingTrigger = false;
			
			// update player position
			playerPosLastTurn = lastKnownPlayerPos;
			lastKnownPlayerPos = GameManager.inst.player.gridPosition;

			allSubjectsPhaseActionTaken = false;
			StartCoroutine(SubjectTakePhaseActions());
		}
		
		return allSubjectsPhaseActionTaken;
	}
	
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
	
	public IEnumerator SubjectTakePhaseActions() {
		bool pAT = true;
		
		for (int i = 0; i < enemyList.Count; i++) {
			if (i == enemyList.Count) {
				yield return null;
			} else {
				yield return new WaitForSeconds(enemyList[i].moveDelayTime);
			}
			
			pAT &= enemyList[i].TakePhaseAction();
		}
		
		// pAT isn't necessary, but may be in the future
		if (pAT) {
			allSubjectsPhaseActionTaken = true;
		}
	}
	
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
}
