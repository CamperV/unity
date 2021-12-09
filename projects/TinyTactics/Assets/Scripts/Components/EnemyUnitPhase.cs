using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyUnit))]
public class EnemyUnitPhase : MonoBehaviour, IUnitPhase
{
    private TurnManager turnManager;
    private EnemyUnit unit;

    public bool active {get; set; } = true;
    public bool moveAvailable { get; set; } = true;
    public bool attackAvailable { get; set; } = true;

    void Awake() {
        turnManager = GetComponentInParent<TurnManager>();
        unit = GetComponent<EnemyUnit>();
    }

    void Start() {
        turnManager.enemyPhase.StartEvent += TriggerPhase;
        turnManager.enemyPhase.EndEvent += EndPhase;
    }

    public void Refresh() {
        active = true;
        moveAvailable = true;
        attackAvailable = true;
    }

    public void TriggerPhase() {
        Refresh();
        unit.ChangeState(EnemyUnit.EnemyUnitFSM.Idle);
    }

    public void EndPhase() {
	}

    public void Finish() {
        active = false;
        moveAvailable = false;
        attackAvailable = false;
        unit.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
    }
}
