using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

[RequireComponent(typeof(UnitCommandSystem))]
public sealed class PlayerUnit : Unit
{
    private UnitCommandSystem unitCommandSystem;

    // cancels movement
    public bool cancelSignal = false;

    // imported from Campaign
    public Guid CampaignID { get; private set; }
    public string UnitName { get; private set; }

    protected override void Awake() {
        base.Awake();
        unitCommandSystem = GetComponent<UnitCommandSystem>();
    }

    public void ImportData(CampaignUnitGenerator.CampaignUnitData unitData) {
        CampaignID = unitData.ID;
        UnitName = unitData.unitName;
        unitStats.ApplyNature(unitData.nature);

        foreach (PerkData perkData in unitData.perks) {
            Type perkType = Type.GetType(perkData.typeName);
            Perk addedPerk = gameObject.AddComponent(perkType) as Perk;
            addedPerk.PerkData = perkData;
        }
    }

    public void OnInteract(GridPosition gp, bool auxiliaryInteract) { 
        if (!turnActive) return;

        unitCommandSystem.Interact(gp, auxiliaryInteract);
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void RevertTurn() {
        unitCommandSystem.CancelActiveCommand();
        return;

        // switch (state) {
        //     case PlayerUnitFSM.Moving:
        //         cancelSignal = true;
        //         break;

        //     case PlayerUnitFSM.MoveSelection:
        //     case PlayerUnitFSM.AttackSelection:
        //         if (turnActive) {
        //             if (moveAvailable == false) UndoMovement();
        //             ChangeState(PlayerUnitFSM.Idle);
        //         }
        //         break;
        // }
    }

    // NOTE: This is janky as hell. Really, I should be using Reservations in the UnitMap, but this kinda works...
    // there theoretically exists a period of time in which things snap around, as MoveUnit can move a Transform, like SpriteAnimator
    // however, the SmoothMovementGrid should override that. I don't know the order of operations vis-a-vis coroutines etc
    //
    // modifies gridPosition & updates threat range
    private void UndoMovement() {
        unitMap.MoveUnit(this, _startingGridPosition);
        _reservedGridPosition = gridPosition;
        statusManager.RemoveAllMovementBuffs();
        RefreshInfo();
    }

    // this needs to run at the end of the frame
    // this is because of our decoupled event processing
    // basically, the PlayerUnits are displaying  before the enemy units drop the display
    //
    // always display AttackRange first, because it is partially overwritten by MoveRange by definition
    public override void DisplayThreatRange() {
        if (moveRange == null || attackRange == null) UpdateThreatRange();
        
        attackRange.Display(battleMap);
        moveRange.Display(battleMap);

    	foreach (GridPosition gp in _ThreatenedRange()) {
			if (moveRange.field.ContainsKey(gp)) {
				battleMap.Highlight(gp, Palette.threatColorIndigo);
			}
		}

        battleMap.Highlight(gridPosition, Palette.selectColorWhite);
    }

    private IEnumerable<GridPosition> _ThreatenedRange() {
		HashSet<GridPosition> threatened = new HashSet<GridPosition>();

		foreach (EnemyUnit enemy in enemyUnitController.activeUnits) {
            if (enemy.attackRange == null) enemy.UpdateThreatRange();
			threatened.UnionWith(enemy.attackRange.field.Keys);
		}

		foreach (GridPosition gp in threatened) yield return gp;
	}

    public override void StartTurn() {
        base.StartTurn();
        unitCommandSystem.SetAllCommandsAvailability(true);
    }

    // diff from Unit.FinishTurn: send signal to the parent controller
    public override void FinishTurn() {
        base.FinishTurn();
        unitCommandSystem.SetAllCommandsAvailability(false);
        playerUnitController.CheckEndPhase();
    }

    public void FinishTurnNoCheck() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteAnimator.SetColor(SpriteAnimator.Inactive);

        FireOnFinishTurnEvent();
    }

    // this is an Action which finishes the unit turn early,
    // / but does so in a way that requires some weird clean up
    public void Wait() {
        FireOnWaitEvent();
        FinishTurn();
    }

    public void WaitNoCheck() {
        FireOnWaitEvent();
        FinishTurnNoCheck();
    }
    
    public void ContextualWait() {
        // if (turnActive) {
        //     if (attackAvailable || (moveAvailable && attackAvailable)) {
        //         switch (state) {
        //             case PlayerUnitFSM.Moving:
        //             case PlayerUnitFSM.Attacking:
        //             case PlayerUnitFSM.Idle:
        //             case PlayerUnitFSM.MoveSelection:
        //                 break;

        //             // only start detecting the Wait signal if you're not animating, etc
        //             case PlayerUnitFSM.AttackSelection:
        //                 ChangeState(PlayerUnitFSM.PreWait);
        //                 break;
        //         }
        //     } else if (moveAvailable && !attackAvailable) {
        //         switch (state) {
        //             case PlayerUnitFSM.Moving:
        //             case PlayerUnitFSM.Attacking:
        //             case PlayerUnitFSM.Idle:
        //                 break;

        //             // only start detecting the Wait signal if you're not animating, etc
        //             case PlayerUnitFSM.MoveSelection:
        //                 ChangeState(PlayerUnitFSM.PreWait);
        //                 break;

        //             case PlayerUnitFSM.AttackSelection:
        //                 break;
        //         }
        //     }
        // }
    }

    public void CancelWait() {
        // if (state == PlayerUnitFSM.PreWait) {
        //     holdTimer.CancelTimer();
            
        //     // since you can only enter PreWait from AttackSelection, head back there
        //     // it will handle itself wrt going to Idle and checking attackAvailable
        //     if (moveAvailable) {
        //         ChangeState(PlayerUnitFSM.MoveSelection);

        //     } else {
        //         ChangeState(PlayerUnitFSM.AttackSelection);
        //     }
        // }
    }

    public void ReservePosition(GridPosition gp) {
        unitMap.ReservePosition(this, gp);
        _reservedGridPosition = gp;
    }

    public void ClaimReservation() {
        unitMap.MoveUnit(this, _reservedGridPosition);
    }
}
