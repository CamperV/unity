using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class TurnManager : MonoBehaviour
{
    public static float endPhaseDelay = 1.0f;

    public Phase playerPhase;
    public Phase enemyPhase;
    public Phase currentPhase;

    public int turnCount = 0;
    public bool enable = true;
    public bool suspend = false;

    public delegate void NewTurn(int newTurn);
    public delegate void NewPhase(Phase newPhase);
    public event NewTurn NewTurnEvent;
    public event NewPhase NewPhaseEvent;

    public PhaseAnnouncement phaseAnnouncementPanel;

    void Awake() {
        playerPhase = new Phase("Player");
        enemyPhase = new Phase("Enemy");
    }

    public void Enable() {
        enable = true;
        StartCoroutine( Loop() );
    }

    // by only touching the enable member, Loop will terminate itself after the current Turn is over
    public void Disable() => enable = false;
    public void Suspend() => suspend = true;
    public void Resume() => suspend = false;

    private IEnumerator Loop() {
        while (enable) {
            turnCount++;
            UIManager.inst.combatLog.AddEntry($"Beginning PURPLE@[Turn {turnCount}].");

            NewTurnEvent?.Invoke(turnCount);
            yield return ExecutePhases(playerPhase, enemyPhase);

            // between turn things happen here
        }
    }

    // start each phase in order, and wait until finished before starting the next
    private IEnumerator ExecutePhases(params Phase[] phases) {
        foreach (Phase phase in phases) {
            // first, check the suspension signal
            // this is different than disabling, which lets all phases play out
            // this will suspend and allow resumption
            if (suspend) yield return new WaitUntil(() => suspend == false);

            string phaseTag = (phase.name == "Player") ? "PLAYER_UNIT" : "ENEMY_UNIT";
            UIManager.inst.combatLog.AddEntry($"Beginning {phaseTag}@[{phase.name}] KEYWORD@[Phase].");

            currentPhase = phase;
            NewPhaseEvent(currentPhase);

            // allow the PhaseAnnouncement banner tot block here,
            yield return AnnounceNewPhase(currentPhase);

            phase.TriggerStart();

            // check for enable here, as you can be disabled when the battle ends
            yield return new WaitUntil(() => phase.state == Phase.PhaseState.Complete || enable == false);

            if (enable) {
                UIManager.inst.combatLog.AddEntry($"Ended {phaseTag}@[{phase.name}] KEYWORD@[Phase].");

                // post-phase delay
                yield return new WaitForSeconds(endPhaseDelay);
            }
        }
    }

	private IEnumerator AnnounceNewPhase(Phase newPhase) {
		phaseAnnouncementPanel.gameObject.SetActive(true);
		
		// OnEnable will also play the sound
		string colorString = (newPhase.name == "Player") ? "#6FD66E" : "#FF6D6D";
		phaseAnnouncementPanel.announcementValue.SetText($"<color={colorString}>{newPhase.name} Phase Start</color>");

		// fade yourself down later
		// this will also resume the attached TurnManager
		yield return new WaitForSeconds(1f);
		yield return phaseAnnouncementPanel.FadeDown(1f);

        phaseAnnouncementPanel.gameObject.SetActive(false);
	}
}