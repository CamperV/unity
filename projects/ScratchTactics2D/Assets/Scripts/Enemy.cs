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
	public int pathRange = 50;
	
	public MoverPath pathToPlayer; // use to cache and not recalculate every frame	
	
	public static Enemy Spawn(Enemy prefab) {
		Vector3 spawnLoc = GameManager.inst.worldGrid.RandomTileReal();
		Enemy enemy = Instantiate(prefab, spawnLoc, Quaternion.identity);
		enemy.ResetPosition(GameManager.inst.worldGrid.Real2GridPos(spawnLoc));
		GameManager.inst.worldGrid.UpdateOccupantAt(enemy.gridPosition, enemy);
		return enemy;
	}
	
    void Awake() {	
		animator = GetComponent<Animator>();
		
		phaseActionTaken = false;
		pathToPlayer = new MoverPath();
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
		if (GameManager.inst.enemyController.HasPlayerMoved() || !pathToPlayer.IsValid()) {
			pathToPlayer.Clear();
			pathToPlayer = GetPathTo(GameManager.inst.player.gridPosition);
		}

		Vector3Int nextStep = ToPosition(pathToPlayer.PopNext(gridPosition), moveSpeed);		
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
		HashSet<Vector3Int> targetPositions = GameManager.inst.worldGrid.GetNeighbors(targetPosition);
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.size != 0) {
			currentPos = pathQueue.Dequeue();
			usedInSearch.Add(currentPos);
			
			// found the target, now recount the path
			if (targetPositions.Contains(currentPos)) {
				cameFrom[targetPosition] = currentPos;
				foundTarget = true;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (Vector3Int adjacent in GameManager.inst.enemyController.GetMovementOptions(currentPos)) {
				if (usedInSearch.Contains(adjacent)) continue;
				cameFrom[adjacent] = currentPos;
				
				// enqueueing based on EdgeCost between two nodes will search correctly
				// but we need to modify such that the prioirty is the total path cost so far
				var totalPathCostSoFar = TotalPathCost(gridPosition, adjacent, cameFrom);
				if (totalPathCostSoFar == -1) continue;
				if (totalPathCostSoFar > pathRange) continue;
				
				pathQueue.Enqueue(CalcPriority(adjacent, targetPosition) + totalPathCostSoFar, adjacent);
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
		} else {
			// we didn't find a valid target/cost was too high. Stay put
			newPathToPlayer.start = gridPosition;
			newPathToPlayer.end   = gridPosition;
			newPathToPlayer.path[gridPosition] = gridPosition;
		}
			
		
		return newPathToPlayer;
	}
	
	private int CalcPriority(Vector3Int src, Vector3Int dest) {
		return (int)Vector3Int.Distance(src, dest);
	}
	
	private int EdgeCost(Vector3Int dest) {
		var destTile = GameManager.inst.worldGrid.GetWorldTileAt(dest);
		if (destTile == null) return -1;
		return destTile.cost;
	}
	
	private int TotalPathCost(Vector3Int src, Vector3Int dest, Dictionary<Vector3Int, Vector3Int> cameFrom) {
		// for now, assume all keys are present
		Vector3Int progenitor = dest;
		int totalCost = 0;
		
		// build the path in reverse, aka next steps (including target)
		while (progenitor != src) {
			var progTile = GameManager.inst.worldGrid.GetWorldTileAt(progenitor);
			if (progTile == null) return -1;
			
			totalCost += progTile.cost;
			progenitor = cameFrom[progenitor];
		}
		return totalCost;
	}

}
