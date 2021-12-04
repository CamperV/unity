using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
public abstract class GridEntity : MonoBehaviour, IGridPosition, IStateMachine<GridEntity.GridEntityFSM>
{
    [field: SerializeField] public GridPosition gridPosition { get; set; }
    public SpriteAnimator spriteAnimator { get; set; }

    public enum GridEntityFSM {
        Idle,
        MoveSelection,
        AttackSelection
    }
    [SerializeField] private GridEntityFSM state = GridEntityFSM.Idle;
    
    // necessary references
    private GridEntityMap gridEntityMap;


    void Awake() {
        gridEntityMap = GetComponentInParent<GridEntityMap>();
    }

    void Start() {
        EnterState(GridEntityFSM.Idle);
    }

    public void ChangeState(GridEntityFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(GridEntityFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case GridEntityFSM.Idle:
            case GridEntityFSM.MoveSelection:
            case GridEntityFSM.AttackSelection:
                break;
        }
        state = GridEntityFSM.Idle;
    }

    public void EnterState(GridEntityFSM enteringState) {
        Debug.Log($"{this} entering state {enteringState}");
        state = enteringState;

        switch (state) {
            case GridEntityFSM.Idle:
            case GridEntityFSM.MoveSelection:
            case GridEntityFSM.AttackSelection:
                break;
        }
    }

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {
            case GridEntityFSM.Idle:
                ChangeState(GridEntityFSM.MoveSelection);
                break;

            case GridEntityFSM.MoveSelection:
                // contextually deselect
                if (gp == gridPosition) {
                    ChangeState(GridEntityFSM.Idle);

                // else if it's a valid movement:
                } else {
                    gridEntityMap.MoveEntity(this, gp);
                    ChangeState(GridEntityFSM.AttackSelection);
                }
                break;

            case GridEntityFSM.AttackSelection:
                // contextually deselect
                if (gp == gridPosition) {
                    ChangeState(GridEntityFSM.Idle);
                } else {
                    Debug.Log($"{this} would like to try to attack {gp}");
                    ChangeState(GridEntityFSM.Idle);
                }
                break;
        }
    }
}
