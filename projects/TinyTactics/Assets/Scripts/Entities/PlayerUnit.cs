using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public sealed class PlayerUnit : Unit
{
    [HideInInspector] public UnitCommandSystem unitCommandSystem;
    [HideInInspector] public ExperienceSystem experienceSystem;

    // imported from Campaign
    public Guid CampaignID { get; private set; }
    public string UnitName { get; private set; }

    protected override void Awake() {
        base.Awake();
        unitCommandSystem = GetComponent<UnitCommandSystem>();
        experienceSystem = GetComponent<ExperienceSystem>();
    }

    protected override void Start() {
        base.Start();
        experienceSystem.Initialize();
        unitCommandSystem.Initialize();
    }

    public void ImportData(CampaignUnitGenerator.CampaignUnitData unitData) {
        CampaignID = unitData.ID;
        UnitName = unitData.unitName;
    }

    public override void OnInteract(GridPosition gp, bool auxiliaryInteract) {
        if (turnActive) unitCommandSystem.Interact(gp, auxiliaryInteract);
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void RevertTurn() {
        unitCommandSystem.CancelActiveCommand();
        unitCommandSystem.RevertExecutedCommands();
    }

    public override void StartTurn() {
        base.StartTurn();
        unitCommandSystem.TickCooldowns();
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

    public void ForfeitReservation() {
        unitMap.MoveUnit(this, _startingGridPosition);
        _reservedGridPosition = gridPosition;
    }
}
