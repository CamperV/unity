using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Mover, IPhasedObject
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	[HideInInspector] public bool phaseActionTaken { get; set; }
	public int moveSpeed = 1;
	
	public MoverPath pathToPlayer; // use to cache and not recalculate every frame	
	
	public static Enemy Spawn(Enemy prefab) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileReal();
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = SpritesResourcesLoader.GetSprite("yellow_skull_red_eyes");
		
		animator = GetComponent<Animator>();
		
		phaseActionTaken = false;
		pathToPlayer = new MoverPath();
		Debug.Log("init new moverpath " + pathToPlayer);
    }
	
    protected override void Start() {
        base.Start();
    }

    void Update() {}
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		// chase player given the coordinates
		// GetIterablePath also sets the List version of the path
		if (GameManager.inst.enemyController.HasPlayerMoved()) {
			pathToPlayer.Clear();
			pathToPlayer = GetPathTo(GameManager.inst.player.gridPosition);
		}

		Vector3Int nextStep = ToPosition(pathToPlayer.Next(gridPosition), moveSpeed);		
		AttemptGridMove(nextStep.x, nextStep.y);
		
		phaseActionTaken = true;
		return phaseActionTaken;
	}
	
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
		
	public void OnHit() {
		animator.SetTrigger("EnemyFlash");
	}
	
	protected override void AttemptGridMove(int xdir, int ydir) {
		base.AttemptGridMove(xdir, ydir);
	}
	
	protected override void OnBlocked<T>(T component) {
		Debug.Log("Enemy collided with " + component);
	}
	
	// AI pathfinding
	// storing this is a hashmap also helps for quickly assessing what squares are available
	private MoverPath GetPathTo(Vector3Int targetPosition) {
		MoverPath newPathToPlayer = new MoverPath();
		
		// this is a simple BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		Vector3Int currentPos = gridPosition;
		
		// track path creation
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		bool foundTarget = false;
		
		HashSet<Vector3Int> usedInSearch = new HashSet<Vector3Int>();
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.size != 0) {
			currentPos = pathQueue.Dequeue();
			usedInSearch.Add(currentPos);
			
			// found the target, now recount the path
			if (currentPos == targetPosition) {
				foundTarget = true;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (Vector3Int adjacent in GameManager.inst.enemyController.GetMovementOptions(currentPos)) {
				if (usedInSearch.Contains(adjacent)) continue;
				cameFrom[adjacent] = currentPos;
				pathQueue.Enqueue(CalcPriority(adjacent, targetPosition), adjacent);
			}
		}
		
		// if we found the target, recount the path to get there
		if (foundTarget) {
			newPathToPlayer.start = gridPosition;
			newPathToPlayer.end   = cameFrom[targetPosition]; // space just outside of the target
			
			// init value only
			Vector3Int progenitor = targetPosition;
			
			while (progenitor != newPathToPlayer.start) {
				var newProgenitor = cameFrom[progenitor];
				
				// build the path in reverse, aka next steps (including target)
				newPathToPlayer.path[newProgenitor] = progenitor;
				progenitor = newProgenitor;
			}
		}
		
		return newPathToPlayer;
	}
	
	private int CalcPriority(Vector3Int src, Vector3Int dest) {
		return (int)Vector3Int.Distance(src, dest);
	}

}
