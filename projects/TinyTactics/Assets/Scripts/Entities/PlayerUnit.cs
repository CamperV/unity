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
    [HideInInspector] public UnitCommandSystem unitCommandSystem;

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
        personalAudioFX.PlayInteractFX();
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
        unitCommandSystem.InitialState();
    }

    // diff from Unit.FinishTurn: send signal to the parent controller
    public override void FinishTurn() {
        base.FinishTurn();
        unitCommandSystem.SetAllCommandsAvailability(false);
        unitCommandSystem.InitialState();
        //
        playerUnitController.CheckEndPhase();
    }

    public void WaitNoCheck() {
        FireOnWaitEvent();

        base.FinishTurn();
        unitCommandSystem.SetAllCommandsAvailability(false);
        unitCommandSystem.InitialState();
    }

    public void ReservePosition(GridPosition gp) {
        unitMap.ReservePosition(this, gp);
        _reservedGridPosition = gp;
    }

    public void ClaimReservation() {
        unitMap.MoveUnit(this, _reservedGridPosition);
    }
}
