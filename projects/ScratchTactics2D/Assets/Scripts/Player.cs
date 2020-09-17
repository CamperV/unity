using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private MoverPath pathToSelected;
	private Queue<Vector3Int> movementQueue;
	
	private bool enqueuedActionsFlag;
	
	[HideInInspector] public bool phaseActionTaken { get; private set; }
	public int moveSpeed = 1;
	
	public static Player Spawn(Player prefab) {
		Player player = Instantiate(prefab, GameManager.inst.worldGrid.RandomTileReal(), Quaternion.identity);
		player.ResetPosition(new Vector3Int(1, 1, 0));
		GameManager.inst.worldGrid.UpdateOccupantAt(player.gridPosition, player);
		return player;
	}

    void Awake() {
		phaseActionTaken = false;
		enqueuedActionsFlag = false;
		movementQueue = new Queue<Vector3Int>();
    }
	
	protected override void Start() {
		base.Start();
		//
		pathToSelected = new MoverPath(gridPosition);
	}

    // Update is called once per frame
    void Update() {
		if (!MyPhase()) return;
		
		// re-calc movement path if necessary
		if (GameManager.inst.mouseManager.HasMouseMoved()) {
			pathToSelected.Clear();
			pathToSelected = MoverPath.GetPathTo<Enemy>(gridPosition, GameManager.inst.mouseManager.currentMouseGridPos, 10000);
			pathToSelected.DrawPath();
		}
		
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.phaseManager.currentPhase == PhaseManager.Phase.player && phaseActionTaken == false;
	}
	
	public void TriggerPhase() {
		phaseActionTaken = false;
	}
	
	public bool TakePhaseAction() {
		// move via pathing, disable input afterwards
		// left mouse button
		// this takes precedent, and can be used to break yourself out of movement
		if (Input.GetMouseButtonDown(0)) {
			movementQueue.Clear();
			
			Vector3Int currPos = gridPosition;
			Vector3Int nextPos = pathToSelected.Next(currPos);
			while (nextPos != pathToSelected.end) {
				movementQueue.Enqueue(Mover.ToPositionStatic(currPos, nextPos, moveSpeed));
				currPos = nextPos;
				nextPos = pathToSelected.Next(nextPos);
			}
			
			enqueuedActionsFlag = true;
			
		// phaseAction possibilities below	
		} else {
			// enqueued actions take precedent over button presses, but not clicks
			// even if the path has become invalidated, go for at least one bump, for good faith
			if (enqueuedActionsFlag) {
				
				//Vector3Int nextStep = actionQueue.PopNext(); // future! learn delegates better
				Vector3Int nextStep = movementQueue.Dequeue();
				AttemptGridMove(nextStep.x, nextStep.y);
				
				// if path becomes invalidated, boot out of enqueued mode
				if (!pathToSelected.IsValid()) {
					pathToSelected.Clear();
					enqueuedActionsFlag = false;
				}
			}
			else if (Input.GetKeyDown("left")  || Input.GetKeyDown(KeyCode.A)) { AttemptGridMove(-1,  0); }
			else if (Input.GetKeyDown("right") || Input.GetKeyDown(KeyCode.D)) { AttemptGridMove( 1,  0); }
			else if (Input.GetKeyDown("up")    || Input.GetKeyDown(KeyCode.W)) { AttemptGridMove( 0,  1); }
			else if (Input.GetKeyDown("down")  || Input.GetKeyDown(KeyCode.S)) { AttemptGridMove( 0, -1); }
			else if (Input.GetKeyDown("space")) { /* pass turn */ }
			else 							    { return false; } // no input taken
		}
		
		// if we've reached this code, the phaseAction has been taken
		// basically, any time Input is recorded a phaseAction is taken
		// if no Input happens, we auto-return before this line
		return true;
	}
		
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
	
	protected override void AttemptGridMove(int xdir, int ydir) {
		base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Enemy hitEnemy = component as Enemy;
		hitEnemy.OnHit();
	}
}
