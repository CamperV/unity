using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerArmyController : Controller
{
	public KeyCode mouseMoveKey;

	// possible actions for Player and their bindings
	private Dictionary<KeyCode, Func<Army, int>> actionBindings = new Dictionary<KeyCode, Func<Army, int>>();
	private Dictionary<Vector3Int, Func<Army, int>> directionMapping = new Dictionary<Vector3Int, Func<Army, int>>();

	public Queue<Func<Army, int>> actionQueue;
	private PlayerArmy registeredPlayer { get => activeRegistry[0] as PlayerArmy; }
	private ArmyPathfinder _pathfinder;
	private OverworldPath _pathToQueue;
	private bool actionQueueEmpty { get => actionQueue.Count == 0; }
	
	protected override void Awake() {
		base.Awake();
		myPhase = Enum.Phase.player;
		actionQueue = new Queue<Func<Army, int>>();
		
		// this needs to be done at run-time
		// is this  abad idiom? Or is this more just for organization?
		actionBindings[KeyCode.LeftArrow]  = MoveLeft;
		actionBindings[KeyCode.RightArrow] = MoveRight;
		actionBindings[KeyCode.UpArrow]    = MoveUp;
		actionBindings[KeyCode.DownArrow]  = MoveDown;
		actionBindings[KeyCode.A]  		   = MoveLeft;
		actionBindings[KeyCode.D] 		   = MoveRight;
		actionBindings[KeyCode.W]    	   = MoveUp;
		actionBindings[KeyCode.S]  		   = MoveDown;
		actionBindings[KeyCode.Space]	   = Pass;

		// for pathToQueue lookup
		directionMapping[Vector3Int.left]  = MoveLeft;
		directionMapping[Vector3Int.right] = MoveRight;
		directionMapping[Vector3Int.up]    = MoveUp;
		directionMapping[Vector3Int.down]  = MoveDown;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.overworld;
	}

	public override void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;

		// update your understanding of what you can and can't path through
		_pathfinder = new ArmyPathfinder(GameManager.inst.enemyController.currentEnemyPositions, registeredPlayer.moveThreshold);
	}
	
	private Vector3Int _prevMousePos;
	void Update() {
		if (!MyPhaseActive()) return;
		KeyCode kc = CheckInput();
		
		// previous cleanup
		if (_prevMousePos != null) GameManager.inst.overworld.ResetOverlayAt(_prevMousePos);
		
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:

				// if you already have actions queued, don't allow for any more queueings
				// this also allows for interupts (i.e. you've been spotted by an enemy, or are blocked)
				if (actionQueueEmpty) {
					// input mode is determined here
					if (Input.GetKey(mouseMoveKey)) {
						Vector3Int mousePos = GameManager.inst.overworld.Real2GridPos(GameManager.inst.mouseManager.mouseWorldPos);

						if (_pathToQueue == null || mousePos != _pathToQueue.end) {
							_pathToQueue?.UnShow();
							_pathToQueue = _pathfinder.NullableBFS(registeredPlayer.gridPosition, mousePos);

							if (_pathToQueue != null) {
								_pathToQueue.interactFlag = Interactable(_pathToQueue.end);
								_pathToQueue.Show();
							} else {
								_prevMousePos = mousePos;
								GameManager.inst.overworld.OverlayAt(mousePos, ScriptableObject.CreateInstance<XOverlayTile>() );
							}
						}
					
						// if the mouse was pressed on the Overworld while in mousemode
						// create an enqueue a series of actions to take based on this path
						if (Input.GetMouseButtonDown(0)) {
							Debug.Assert(_pathToQueue != null);

							foreach (Vector3Int nextMove in _pathToQueue.Serially()) {
								actionQueue.Enqueue(directionMapping[nextMove]);
							}
						}

					// or, if there was a relevant keypress
					} else if (actionBindings.ContainsKey(kc)) {
						actionQueue.Enqueue(actionBindings[kc]);

						_pathToQueue?.UnShow();
						_pathToQueue = null;
					} else {
						_pathToQueue?.UnShow();
						_pathToQueue = null;
					}
				}

				// NOTE: this is NOT in an else clause
				// this is because we must be able to populate and execute the queue in the same frame
				if (!actionQueueEmpty) {
					Func<Army, int> actionToTake = actionQueue.Dequeue();

					phaseActionState = Enum.PhaseActionState.acting;
					PlayerTakeAction(actionToTake);

					// finally, start "consuming" the displayed path
					if (!_pathToQueue?.IsEmpty() ?? false) {
						_pathToQueue.UnShowUntil(registeredPlayer.gridPosition);
					}
				}
				break;
				
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase();
				break;
			
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:	
			case Enum.PhaseActionState.postPhase:
				break;
		}
    }

	public void ClearActionQueue() {
		actionQueue.Clear();
		_pathToQueue?.UnShow();
		_pathToQueue = null;
	}

	private KeyCode CheckInput() {
		// return KeyCode that is down, checking in "actionBindings" order
		foreach (KeyCode kc in actionBindings.Keys) {
			if (Input.GetKeyDown(kc)) return kc; 
		}
		return KeyCode.None;
	}

	private void PlayerTakeAction(Func<Army, int> Action) {
		// we've taken an action... but what did it cost
		int ticksTaken = Action(registeredPlayer);
		GameManager.inst.enemyController.AddTicksAll(ticksTaken);

		// this delays key presses too much, maybe
		StartCoroutine( registeredPlayer.spriteAnimator.ExecuteAfterMoving(() => {
			phaseActionState = Enum.PhaseActionState.complete;
		}) );
	}

	// these Funcs return the cost of taking said actions
	private int MoveLeft(Army subject) {
		var success = subject.GridMove(-1, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveRight(Army subject) {
		var success = subject.GridMove(1, 0);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveUp(Army subject) {
		var success = subject.GridMove(0, 1);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int MoveDown(Army subject) {
		var success = subject.GridMove(0, -1);
		var tickCost = GameManager.inst.overworld.TerrainAt(subject.gridPosition).tickCost;
		return (success) ? (int)(tickCost / subject.moveSpeed) : Constants.standardTickCost;
	}
	private int Pass(Army subject) {
		return Constants.standardTickCost;
	}

	private bool Interactable(Vector3Int v) {
		HashSet<Vector3Int> canInteractWith = new HashSet<Vector3Int>();
		canInteractWith.UnionWith(GameManager.inst.enemyController.currentEnemyPositions);
		return canInteractWith.Contains(v);
	}
}