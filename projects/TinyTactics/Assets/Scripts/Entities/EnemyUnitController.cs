using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitController : MonoBehaviour, IUnitPhaseController
{
    public delegate void RegistrationState(Unit unit);
    public event RegistrationState RegisteredUnit;

    public static float timeBetweenUnitActions = 0.75f; // seconds

    [SerializeField] private List<EnemyUnit> _activeUnits;
    public List<EnemyUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<EnemyUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    private PlayerUnitController playerUnitController;
    private BattleMap battleMap;

    public bool cancelSignal;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
        battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        foreach (EnemyUnit en in GetComponentsInChildren<EnemyUnit>()) {
            _activeUnits.Add(en);
            RegisteredUnit?.Invoke( (en as Unit) );
        }
    }

    public void TriggerPhase() {
        foreach (EnemyUnit unit in activeUnits) {
            unit.RefreshTargets();
            unit.StartTurn();
        }

        // also, update the threat ranges of the pods
        // we can't have this update every time a unit takes a turn
        BrainPod[] pods = GetComponentsInChildren<BrainPod>();
        foreach (BrainPod pod in pods) {
            pod.UpdateSharedDetectionRange();
        }

        StartCoroutine( TakeActionAll() );
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    public void EndPhase() {}

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

    public void Preview(EnemyUnit selection) {
        selection.UpdateThreatRange();
        selection.DisplayThreatRange();
        selection.personalAudioFX.PlayWakeUpFX();
    }

    public void ClearPreview() {
        battleMap.ResetHighlightTiles();
        battleMap.ResetHighlight();
    }

	private IEnumerator TakeActionAll() {
        // first, try for any available Pods
        // they take their turns together, then take the stragglers
        BrainPod[] pods = GetComponentsInChildren<BrainPod>();
        foreach (BrainPod pod in pods.OrderBy(pod => pod.Initiative)) {

            foreach (EnemyUnit unit in pod.podmates.Where(it => it.turnActive).OrderBy(unit => unit.Initiative)) {
                // if you've been cancelled, say by the Battle ending/turnManager suspending
                if (cancelSignal) yield break;

                yield return TakeAction(unit);
            }
        }

        // then, if the pods have gone, collect the stragglers (if their turn is still active)
        foreach (EnemyUnit unit in activeUnits.Where(it => it.turnActive).OrderBy(unit => unit.Initiative)) {
            // if you've been cancelled, say by the Battle ending/turnManager suspending
            if (cancelSignal) yield break;

            yield return TakeAction(unit);
        }

        GetComponentInParent<TurnManager>().enemyPhase.TriggerEnd();
	}

    private IEnumerator TakeAction(EnemyUnit unit) {        
        // wait until the unit says you can move on
        // generally this is until the unit's turn is over,
        // but if the unit is only moving (and not attacking), just execute the next unit's whole situation
        yield return new WaitUntil(() => !unit.spriteAnimator.isAnimating);

        EnemyBrain.DamagePackage? selectedDmgPkg;
        Path<GridPosition> pathTo;
        unit.SelectDamagePackage(out selectedDmgPkg, out pathTo);

        // if the unit wants to end early, let them
        // ie, unit can't actually execute the DamagePackage it wants to
        if (selectedDmgPkg == null) {
            unit.FinishTurn();

            // uncomment to focus the camera when they decide not to move            
            // focus camera here
            // yield return new WaitForSeconds(timeBetweenUnitActions/4f);

        // otherwise, if they want to take an action:
        // focus on it (camera)
        // execute the dang thing
        // and wait the normal amount between turns
        } else {
            //
            // focus camera here
            //
            //
            yield return unit.ExecuteDamagePackage(selectedDmgPkg.Value, pathTo);
            yield return new WaitForSeconds(timeBetweenUnitActions);
        }
    }
}