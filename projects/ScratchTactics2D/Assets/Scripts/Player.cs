using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	private MovingObjectPath pathToSelected;
	private Queue<Vector3Int> movementQueue;
	
	public int moveSpeed = 1;
	public int pathRange = Int32.MaxValue;
	
	public static Player Spawn(Player prefab) {
		Player player = Instantiate(prefab, GameManager.inst.worldGrid.RandomTileReal(), Quaternion.identity);
		player.ResetPosition(new Vector3Int(1, 1, 0));
		GameManager.inst.worldGrid.UpdateOccupantAt(player.gridPosition, player);
		return player;
	}

    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		//
		myPhase = Enum.Phase.player;
		movementQueue = new Queue<Vector3Int>();
    }
	
	protected override void Start() {
		base.Start();
		//
		pathToSelected = new MovingObjectPath(gridPosition);
	}

    // Update is called once per frame
    void Update() {
		if (!MyPhaseActive()) return;
		
		// re-calc movement path if necessary
		if (movementQueue.Count == 0 && GameManager.inst.mouseManager.HasMouseMoved()) {
			pathToSelected.Clear();
			GameManager.inst.worldGrid.ClearTilesOnLevel(Enum.TileLevel.overlay);
			//
			pathToSelected = MovingObjectPath.GetPathTo(gridPosition, GameManager.inst.mouseManager.currentMouseGridPos, pathRange);
			pathToSelected.DrawPath();
		}
		
		// looks for input, returns true if input is taken otherwise spins
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
	
	public override void TakePhaseAction() {
		// move via pathing, disable input afterwards
		// left mouse button
		// this takes precedent, and can be used to break yourself out of movement
		if (Input.GetMouseButtonDown(0)) {
			movementQueue.Clear();
			
			foreach (Vector3Int nextMove in pathToSelected.GetPathEdges()) {
				movementQueue.Enqueue(nextMove);
			}
		}
			
		// phaseAction possibilities below	
		// enqueued actions take precedent over button presses, but not clicks
		// even if the path has become invalidated, go for at least one bump, for good faith
		if (movementQueue.Count != 0) {
			phaseActionState = Enum.PhaseActionState.acting;
			
			Vector3Int nextStep = movementQueue.Dequeue();
			var moveSuccess = AttemptGridMove(nextStep.x, nextStep.y);
			
			// if path becomes invalidated, boot out of enqueued mode
			if (!moveSuccess) {
				pathToSelected.Clear();
				movementQueue.Clear();
			} else {
				// if the move was successful, remove it from the pathToSelected
				pathToSelected.Consume(nextStep);
			}
		}
		
		// <-
		else if (Input.GetKeyDown("left") || Input.GetKeyDown(KeyCode.A)) {
			phaseActionState = Enum.PhaseActionState.acting;
			AttemptGridMove(-1, 0);
		}
		
		// ->
		else if (Input.GetKeyDown("right") || Input.GetKeyDown(KeyCode.D)) {
			phaseActionState = Enum.PhaseActionState.acting;
			AttemptGridMove(1, 0);
		}
		
		// ^
		else if (Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W)) {
			phaseActionState = Enum.PhaseActionState.acting;
			AttemptGridMove(0, 1);
		}
		
		// v
		else if (Input.GetKeyDown("down") || Input.GetKeyDown(KeyCode.S)) {
			phaseActionState = Enum.PhaseActionState.acting;
			AttemptGridMove(0, -1);
		}
		
		// pass
		else if (Input.GetKeyDown("space")) {
			phaseActionState = Enum.PhaseActionState.acting;
			/* pass turn */
			
		// if you received no input, continue .waitingForInput
		} else { return; }
		
		// if you've reached this code, you've accepted input, and completed your move action
		phaseActionState = Enum.PhaseActionState.complete;
	}
		
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
	
	protected override bool AttemptGridMove(int xdir, int ydir) {
		return base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Enemy hitEnemy = component as Enemy;
		hitEnemy.OnHit();
	}
}
